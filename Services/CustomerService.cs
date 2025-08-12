using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Services
{
    public class CustomerService
    {
        private readonly string connectionString;

        public CustomerService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Finds a single customer by their ID or name.
        /// This method is used on the Process Rental page.
        /// </summary>
        /// <param name="searchTerm">The ID or name to search for.</param>
        /// <returns>A Customer object if found; otherwise, null.</returns>
        public async Task<Customer> FindCustomerAsync(string searchTerm)
        {
            Customer customer = null;
            string query;

            // This 'using' block ensures the database connection is always closed,
            // even if something goes wrong. It's a very important pattern.
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Check if the search term is a number (for ID) or text (for name).
                if (int.TryParse(searchTerm, out int customerId))
                {
                    // If it's a number, search by the ID column.
                    query = "SELECT * FROM Customers WHERE Id = @SearchTerm";
                }
                else
                {
                    // If it's text, search in the FirstName or LastName columns.
                    // The '%' are wildcards, so a search for "John" will match "John Doe".
                    query = "SELECT * FROM Customers WHERE FirstName LIKE @SearchTerm OR LastName LIKE @SearchTerm";
                    searchTerm = $"%{searchTerm}%";
                }

                using (var command = new SqlCommand(query, connection))
                {
                    // Add the search term as a parameter. This is a security best practice
                    // to prevent a type of attack called SQL Injection.
                    command.Parameters.AddWithValue("@SearchTerm", searchTerm);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // We only expect one result, so we use 'if' instead of 'while'.
                        if (await reader.ReadAsync())
                        {
                            // Create a new Customer object and fill it with data from the database row.
                            customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                IsBanned = reader.GetBoolean(reader.GetOrdinal("IsBanned")),
                                DiscountRate = (double)reader.GetDecimal(reader.GetOrdinal("DiscountRate"))
                            };
                        }
                    }
                }
            }
            return customer;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customers = new List<Customer>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Customers ORDER BY LastName, FirstName";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            customers.Add(new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                IsBanned = reader.GetBoolean(reader.GetOrdinal("IsBanned")),
                                DiscountRate = (double)reader.GetDecimal(reader.GetOrdinal("DiscountRate"))
                            });
                        }
                    }
                }
            }
            return customers;
        }

        public async Task<bool> AddCustomerAsync(Customer customer)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    INSERT INTO Customers (FirstName, LastName, PhoneNumber, Email, IsBanned, DiscountRate) 
                    VALUES (@FirstName, @LastName, @PhoneNumber, @Email, @IsBanned, @DiscountRate)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", customer.FirstName);
                    command.Parameters.AddWithValue("@LastName", customer.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    command.Parameters.AddWithValue("@IsBanned", customer.IsBanned);
                    command.Parameters.AddWithValue("@DiscountRate", customer.DiscountRate);

                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0; 
                }
            }
        }
    }
}
