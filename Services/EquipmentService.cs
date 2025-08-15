using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models; // Make sure this points to your models

namespace VillageRentalManagementSystem.Services
{
    public class EquipmentService
    {
        private readonly string connectionString;

        public EquipmentService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<Equipment> FindEquipmentAsync(string searchTerm)
        {
            Equipment equipment = null;
            string query;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                if (int.TryParse(searchTerm, out int equipmentId))
                {
                    // Search by the primary key ID
                    query = "SELECT * FROM Equipment WHERE Id = @SearchTerm";
                }
                else
                {
                    // Search by the name of the equipment
                    query = "SELECT * FROM Equipment WHERE Name LIKE @SearchTerm";
                    searchTerm = $"%{searchTerm}%"; //wildcards for partial matches
                }

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", searchTerm);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            equipment = new Equipment
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                DailyRentalCost = (double)reader.GetDecimal(reader.GetOrdinal("DailyRentalCost")),
                                IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable"))
                            };
                        }
                    }
                }
            }
            return equipment;
        }

        public async Task<List<Equipment>> GetAllEquipmentAsync()
        {
            var equipmentList = new List<Equipment>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT eq.Id, eq.Name, eq.Description, eq.DailyRentalCost, eq.IsAvailable, cat.Name AS CategoryName
                    FROM Equipment eq
                    LEFT JOIN Categories cat ON eq.CategoryId = cat.Id
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
                                //create a new Category object for the equipment
                                Category = new Category(0, reader.GetString(reader.GetOrdinal("CategoryName")))
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
                    command.Parameters.AddWithValue("@CategoryId", equipment.Category.Id);

                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        public async Task<bool> DeleteEquipmentAsync(int equipmentId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Equipment WHERE Id = @EquipmentId";
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
