using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models; 

namespace VillageRentalManagementSystem.Services
{
    public class CategoryService
    {
        private readonly string _connectionString;

        public CategoryService()
        {
            _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=VillageRentalDB;Trusted_Connection=True;";

        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var categories = new List<Category>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT Id, Name FROM Categories ORDER BY Id";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // For each row in the result, create a new Category object
                            // and add it to our list.
                            categories.Add(new Category(
                                id: reader.GetInt32(reader.GetOrdinal("Id")),
                                name: reader.GetString(reader.GetOrdinal("Name"))
                            ));
                        }
                    }
                }
            }
            return categories;
        }

        //Adds a new category to the database
        public async Task<bool> AddCategoryAsync(Category category)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "INSERT INTO Categories (Name) VALUES (@Name)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", category.Name);

                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }


        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE Categories SET Name = @Name WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@Id", category.Id);

                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Categories WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", categoryId);

                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
    }
}
