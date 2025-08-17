using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
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
        }

        public async Task<bool> CreateRentalAsync(Rental rental)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    var rentalInsertQuery = @"
                        INSERT INTO Rentals (CustomerId, RentalDate, ExpectedReturnDate, TotalCost) 
                        VALUES (@CustomerId, @RentalDate, @ExpectedReturnDate, @TotalCost);
                        SELECT SCOPE_IDENTITY();";

                    int newRentalId;
                    using (var command = new SqlCommand(rentalInsertQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@CustomerId", rental.customer.Id);
                        command.Parameters.AddWithValue("@RentalDate", DateTime.Now);
                        command.Parameters.AddWithValue("@ExpectedReturnDate", rental.ExpectedReturnDate);
                        command.Parameters.AddWithValue("@TotalCost", rental.CalculateTotalCost());
                        var result = await command.ExecuteScalarAsync();
                        newRentalId = Convert.ToInt32(result);
                        rental.Id = newRentalId;
                    }

                    foreach (var item in rental.items)
                    {
                        // CORRECTED: Added ExpectedReturnDate to the INSERT statement
                        var rentalItemInsertQuery = @"
                            INSERT INTO RentalItems (RentalId, EquipmentId, RentalCost, ExpectedReturnDate) 
                            VALUES (@RentalId, @EquipmentId, @RentalCost, @ExpectedReturnDate)";

                        using (var command = new SqlCommand(rentalItemInsertQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@RentalId", newRentalId);
                            command.Parameters.AddWithValue("@EquipmentId", item.equipment.Id);
                            command.Parameters.AddWithValue("@RentalCost", item.rentalCost);
                            command.Parameters.AddWithValue("@ExpectedReturnDate", item.ExpectedReturnDate); // Added this parameter
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
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<List<Rental>> GetAllRentalAsync()
        {
            var rentalList = new List<Rental>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT DISTINCT r.Id AS RentalId, r.RentalDate, r.ExpectedReturnDate, r.TotalCost, 
                                    c.Id AS CustomerId, c.FirstName, c.LastName, c.Email
                    FROM Rentals r
                    JOIN Customers c ON r.CustomerId = c.Id
                    JOIN RentalItems ri ON r.Id = ri.RentalId
                    WHERE ri.IsReturned = 0
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
