using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Services
{
    public class CustomerService
    {
        private readonly string _connectionString;

        public CustomerService()
        {
            _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=VillageRentalDB;Trusted_Connection=True;";
        }

        public async Task<Customer> FindCustomerAsync(string searchTerm)
        {
            Customer customer = null;
            string query;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                if (int.TryParse(searchTerm, out int customerId))
                {
                    query = "SELECT * FROM Customers WHERE Id = @SearchTerm";
                }
                else
                {
                    query = "SELECT * FROM Customers WHERE FirstName LIKE @SearchTerm OR LastName LIKE @SearchTerm";
                    searchTerm = $"%{searchTerm}%";
                }

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", searchTerm);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
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
            List<Customer> customers = new List<Customer>();
            using (var connection = new SqlConnection(_connectionString))
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
            using (var connection = new SqlConnection(_connectionString))
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

        // NEW: Method to update an existing customer
        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    UPDATE Customers 
                    SET FirstName = @FirstName, 
                        LastName = @LastName, 
                        PhoneNumber = @PhoneNumber, 
                        Email = @Email, 
                        IsBanned = @IsBanned, 
                        DiscountRate = @DiscountRate
                    WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", customer.Id);
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

        // NEW: Method to delete a customer
        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Customers WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", customerId);
                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
    }
}
