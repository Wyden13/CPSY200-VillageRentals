using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Exceptions; // Import the new namespace
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

        private void ValidateCustomer(Customer customer)
        {
            // Regex for a standard email format
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            if (!emailRegex.IsMatch(customer.Email))
            {
                throw new InvalidFormatException("Invalid email format. Please use a valid email address (e.g., user@example.com).");
            }

            // Regex for a simple North American phone number format
            var phoneRegex = new Regex(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$");
            if (!phoneRegex.IsMatch(customer.PhoneNumber))
            {
                throw new InvalidFormatException("Invalid phone number format. Please use a valid 10-digit phone number (e.g., 555-555-5555).");
            }
        }

        public async Task<bool> AddCustomerAsync(Customer customer)
        {
            ValidateCustomer(customer);

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

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            ValidateCustomer(customer); 

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    UPDATE Customers 
                    SET FirstName = @FirstName, LastName = @LastName, PhoneNumber = @PhoneNumber, 
                        Email = @Email, IsBanned = @IsBanned, DiscountRate = @DiscountRate
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

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customers = new List<Customer>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Customers WHERE IsDeleted = 0 ORDER BY LastName, FirstName";
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
                                DiscountRate = (double)reader.GetDecimal(reader.GetOrdinal("DiscountRate")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
                            });
                        }
                    }
                }
            }
            return customers;
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE Customers SET IsDeleted = 1 WHERE Id = @Id";
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
