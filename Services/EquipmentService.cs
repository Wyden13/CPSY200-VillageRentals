using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Services
{
    public class EquipmentService
    {
        private readonly string connectionString;

        public EquipmentService()
        {
            connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=VillageRentalDB;Trusted_Connection=True;";
        }

        public async Task<List<Equipment>> GetAllEquipmentAsync()
        {
            var equipmentList = new List<Equipment>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                // UPDATED: Added "WHERE eq.IsDeleted = 0"
                var query = @"
                    SELECT eq.Id, eq.Name, eq.Description, eq.DailyRentalCost, eq.IsAvailable, eq.IsDeleted, eq.CategoryId, cat.Name AS CategoryName
                    FROM Equipment eq
                    LEFT JOIN Categories cat ON eq.CategoryId = cat.Id
                    WHERE eq.IsDeleted = 0
                    ORDER BY eq.Name";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var equipment = new Equipment
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                DailyRentalCost = (double)reader.GetDecimal(reader.GetOrdinal("DailyRentalCost")),
                                IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                                Category = new Category
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("CategoryId")) ? 0 : reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? "Uncategorized" : reader.GetString(reader.GetOrdinal("CategoryName"))
                                }
                            };
                            equipmentList.Add(equipment);
                        }
                    }
                }
            }
            return equipmentList;
        }

        public async Task<List<Equipment>> GetAllAvailableEquipmentAsync()
        {
            var equipmentList = new List<Equipment>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                // UPDATED: Added "AND eq.IsDeleted = 0"
                var query = @"
                    SELECT eq.Id, eq.Name, eq.Description, eq.DailyRentalCost, eq.IsAvailable, eq.CategoryId, cat.Name AS CategoryName
                    FROM Equipment eq
                    LEFT JOIN Categories cat ON eq.CategoryId = cat.Id
                    WHERE eq.IsAvailable = 1 AND eq.IsDeleted = 0
                    ORDER BY eq.Name";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var equipment = new Equipment
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                DailyRentalCost = (double)reader.GetDecimal(reader.GetOrdinal("DailyRentalCost")),
                                IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                                Category = new Category { Name = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? "Uncategorized" : reader.GetString(reader.GetOrdinal("CategoryName")) }
                            };
                            equipmentList.Add(equipment);
                        }
                    }
                }
            }
            return equipmentList;
        }

        public async Task<bool> AddEquipmentAsync(Equipment equipment)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    INSERT INTO Equipment (Name, Description, DailyRentalCost, IsAvailable, CategoryId) 
                    VALUES (@Name, @Description, @DailyRentalCost, @IsAvailable, @CategoryId)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", equipment.Name);
                    command.Parameters.AddWithValue("@Description", equipment.Description);
                    command.Parameters.AddWithValue("@DailyRentalCost", equipment.DailyRentalCost);
                    command.Parameters.AddWithValue("@IsAvailable", equipment.IsAvailable);
                    command.Parameters.AddWithValue("@CategoryId", (object)equipment.Category?.Id ?? DBNull.Value);

                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> UpdateEquipmentAsync(Equipment equipment)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    UPDATE Equipment 
                    SET Name = @Name, Description = @Description, DailyRentalCost = @DailyRentalCost, CategoryId = @CategoryId
                    WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", equipment.Id);
                    command.Parameters.AddWithValue("@Name", equipment.Name);
                    command.Parameters.AddWithValue("@Description", equipment.Description);
                    command.Parameters.AddWithValue("@DailyRentalCost", equipment.DailyRentalCost);
                    command.Parameters.AddWithValue("@CategoryId", (object)equipment.Category?.Id ?? DBNull.Value);

                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        // This is now a soft delete
        public async Task<bool> DeleteEquipmentAsync(int equipmentId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE Equipment SET IsDeleted = 1 WHERE Id = @EquipmentId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EquipmentId", equipmentId);
                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
    }
}
