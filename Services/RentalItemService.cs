using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Services
{
    public class RentalItemService
    {
        private readonly string _connectionString;

        public RentalItemService()
        {
            _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=VillageRentalDB;Trusted_Connection=True;";
        }

        public async Task<List<RentalItem>> GetRentalItemsAsync(int rentalId)
        {
            var rentalItems = new List<RentalItem>();
            var query = @"
                SELECT 
                    ri.Id, ri.RentalId, ri.RentalCost, ri.IsReturned, ri.ActualReturnDate, ri.ExpectedReturnDate,
                    eq.Id AS EquipmentId, eq.Name AS EquipmentName
                FROM RentalItems ri
                JOIN Equipment eq ON ri.EquipmentId = eq.Id
                WHERE ri.RentalId = @RentalId";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RentalId", rentalId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new RentalItem
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                RentalId = reader.GetInt32(reader.GetOrdinal("RentalId")),
                                rentalCost = (double)reader.GetDecimal(reader.GetOrdinal("RentalCost")),
                                IsReturned = reader.GetBoolean(reader.GetOrdinal("IsReturned")),
                                ActualReturnDate = reader.IsDBNull(reader.GetOrdinal("ActualReturnDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ActualReturnDate")),
                                ExpectedReturnDate = reader.GetDateTime(reader.GetOrdinal("ExpectedReturnDate")), // Added this line
                                equipment = new Equipment
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EquipmentId")),
                                    Name = reader.GetString(reader.GetOrdinal("EquipmentName"))
                                }
                            };
                            rentalItems.Add(item);
                        }
                    }
                }
            }
            return rentalItems;
        }

        public async Task<bool> ReturnRentalItemAsync(int rentalItemId, int equipmentId)
        {
            var query = @"
                UPDATE RentalItems
                SET IsReturned = 1, ActualReturnDate = @ReturnDate
                WHERE Id = @RentalItemId;

                UPDATE Equipment
                SET IsAvailable = 1
                WHERE Id = @EquipmentId;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReturnDate", DateTime.Now);
                    command.Parameters.AddWithValue("@RentalItemId", rentalItemId);
                    command.Parameters.AddWithValue("@EquipmentId", equipmentId);

                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
    }
}
