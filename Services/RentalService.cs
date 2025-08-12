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

        public RentalService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
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

                        // Use ExecuteScalarAsync because we expect a single value back (the new ID).
                        var result = await command.ExecuteScalarAsync();
                        newRentalId = Convert.ToInt32(result);
                        rental.Id = newRentalId; // Assign the new ID back to our object.
                    }

                    // STEP 2: Loop through each item in the shopping cart and process it.
                    foreach (var item in rental.items)
                    {
                        // STEP 2a: Insert a record into the 'RentalItems' linking table.
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

                        // STEP 2b: Update the equipment's status to make it unavailable.
                        var equipmentUpdateQuery = "UPDATE Equipment SET IsAvailable = 0 WHERE Id = @EquipmentId";
                        using (var command = new SqlCommand(equipmentUpdateQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@EquipmentId", item.equipment.Id);
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    // If all the above commands executed without any errors,
                    // 'Commit' the transaction to save all the changes permanently.
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // If ANY error occurred during the 'try' block, we immediately jump here.
                    Console.WriteLine($"An error occurred: {ex.Message}");

                    // 'Rollback' the transaction. This undoes ALL the changes made since
                    // we started the transaction, leaving the database in a clean state.
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}
