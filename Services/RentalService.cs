using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Services
{
    public class RentalService
    {
        private readonly string connectionString;

        public RentalService()
        {
            connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=VillageRentalDB;Integrated Security=True";
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
                        INSERT INTO Rentals (CustomerId, RentalDate, TotalCost) 
                        VALUES (@CustomerId, @RentalDate, @TotalCost);
                        SELECT SCOPE_IDENTITY();";

                    int newRentalId;
                    using (var command = new SqlCommand(rentalInsertQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@CustomerId", rental.customer.Id);
                        command.Parameters.AddWithValue("@RentalDate", DateTime.Now);
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
    }
}
