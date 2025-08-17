using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Services
{
    public class RentalService
    {
        private readonly string connectionString;

        public RentalService()
        {
            connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=VillageRentalDB;Trusted_Connection=True;";
            //?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
        }

        public async Task<bool> CreateRentalAsync(Rental rental)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // 'SCOPE_IDENTITY()' is a special SQL command. It gets the ID of the
                    // row we just inserted.
                    var rentalInsertQuery = @"
                        INSERT INTO Rentals (CustomerId, RentalDate, ExpectedReturnDate, TotalCost) 
                        VALUES (@CustomerId, @RentalDate, @ExpectedReturnDate, @TotalCost);
                        SELECT SCOPE_IDENTITY();";

                    int newRentalId;
                    using (var command = new SqlCommand(rentalInsertQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@CustomerId", rental.customer.Id);
                        command.Parameters.AddWithValue("@RentalDate", DateTime.Now);
                        command.Parameters.AddWithValue("@ExpectedReturnDate", rental.items[0].returnDAte);
                        command.Parameters.AddWithValue("@TotalCost", rental.CalculateTotalCost());
                        var result = await command.ExecuteScalarAsync();
                        newRentalId = Convert.ToInt32(result);
                        rental.Id = newRentalId; 
                    }

                    foreach (var item in rental.items)
                    {
                        var rentalItemInsertQuery = @"
                            INSERT INTO RentalItems (RentalId, EquipmentId, RentalCost) 
                            VALUES (@RentalId, @EquipmentId, @RentalCost)";

                        using (var command = new SqlCommand(rentalItemInsertQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@RentalId", newRentalId);
                            command.Parameters.AddWithValue("@EquipmentId", item.equipment.Id);
                            command.Parameters.AddWithValue("@RentalCost", item.rentalCost);
                            await command.ExecuteNonQueryAsync();
                        }

                        var equipmentUpdateQuery = "UPDATE Equipment SET IsAvailable = 0 WHERE Id = @EquipmentId";
                        using (var command = new SqlCommand(equipmentUpdateQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@EquipmentId", item.equipment.Id);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");

                    // 'Rollback' undoes ALL the changes made since
                    // we started the transaction, leaving the database in a clean state.
                    transaction.Rollback();
                    return false;
                }
            }
        }
        public async Task<bool> ReturnRentalAsync(int rentalId)
        {
            var query = @"
                -- Update the rental's return date.
                UPDATE Rentals 
                SET ActualReturnDate = @ReturnDate 
                WHERE Id = @RentalId;

                -- Update the availability of the equipment from that rental.
                UPDATE Equipment
                SET IsAvailable = 1
                WHERE Id IN (
                    SELECT EquipmentId 
                    FROM RentalItems 
                    WHERE RentalId = @RentalId
                );";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReturnDate", DateTime.Today);
                    command.Parameters.AddWithValue("@RentalId", rentalId);

                    await command.ExecuteNonQueryAsync();
                }

            }

            return true;
        }

        public async Task<List<Rental>> GetAllRentalAsync()
        {
            var rentalList= new List<Rental>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT r.Id AS RentalId, r.RentalDate, r.ExpectedReturnDate, r.TotalCost, r.CustomerId AS CustomerId, c.FirstName, c.LastName, c.Email
                    FROM Rentals r
                    JOIN Customers c ON r.CustomerId = c.Id
                    WHERE r.ActualReturnDate IS NULL
                    ORDER BY r.ExpectedReturnDate";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var rental = new Rental
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("RentalId")),
                                customer = new Customer
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                        Email = reader.GetString(reader.GetOrdinal("Email")),
                                    },
                                RentalDate = reader.GetDateTime(reader.GetOrdinal("RentalDate")),
                                ExpectedReturnDate = reader.IsDBNull(reader.GetOrdinal("ExpectedReturnDate")) ? DateTime.MaxValue : reader.GetDateTime(reader.GetOrdinal("ExpectedReturnDate"))
                            };
                            rental.TotalCost = (double)reader.GetDecimal(reader.GetOrdinal("TotalCost"));
                            rentalList.Add(rental);
                        }
                    }
                }
            }
            return rentalList;
        }
    }
}
