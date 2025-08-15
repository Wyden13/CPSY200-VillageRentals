using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;
using VillageRentalManagementSystem.Models.Reports; // Use the new namespace for our report models

namespace VillageRentalManagementSystem.Services
{
    /// <summary>
    /// This service class handles generating reports by running complex queries
    /// against the database.
    /// </summary>
    public class ReportService
    {
        private readonly string _connectionString;

        public ReportService()
        {
            _connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=VillageRentalDB;Integrated Security=True";
        }

        /// <summary>
        /// Generates a sales report, summarizing total rental income per day
        /// within a specified date range.
        /// </summary>
        public async Task<List<DailySalesReportItem>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var reportData = new List<DailySalesReportItem>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT 
                        CAST(RentalDate AS DATE) AS SaleDate, 
                        SUM(TotalCost) AS TotalDailySales
                    FROM Rentals
                    WHERE RentalDate >= @StartDate AND RentalDate < @EndDatePlusOne
                    GROUP BY CAST(RentalDate AS DATE)
                    ORDER BY SaleDate;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate.Date);
                    command.Parameters.AddWithValue("@EndDatePlusOne", endDate.Date.AddDays(1));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reportData.Add(new DailySalesReportItem
                            {
                                Date = reader.GetDateTime(reader.GetOrdinal("SaleDate")),
                                TotalSales = reader.GetDecimal(reader.GetOrdinal("TotalDailySales"))
                            });
                        }
                    }
                }
            }
            return reportData;
        }

        /// <summary>
        /// Generates a report summarizing total sales for each customer.
        /// </summary>
        public async Task<List<CustomerSalesReportItem>> GetSalesByCustomerAsync()
        {
            var reportData = new List<CustomerSalesReportItem>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // This query joins Rentals and Customers to get the customer's name,
                // then groups by customer to sum up their total spending.
                var query = @"
                    SELECT 
                        c.Id AS CustomerId,
                        c.FirstName + ' ' + c.LastName AS CustomerName,
                        SUM(r.TotalCost) as TotalAmountSpent
                    FROM Rentals r
                    JOIN Customers c ON r.CustomerId = c.Id
                    GROUP BY c.Id, c.FirstName, c.LastName
                    ORDER BY TotalAmountSpent DESC;";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reportData.Add(new CustomerSalesReportItem
                            {
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                                TotalAmountSpent = reader.GetDecimal(reader.GetOrdinal("TotalAmountSpent"))
                            });
                        }
                    }
                }
            }
            return reportData;
        }

        /// <summary>
        /// Generates a report summarizing total sales for each equipment category.
        /// </summary>
        public async Task<List<CategorySalesReportItem>> GetSalesByCategoryAsync()
        {
            var reportData = new List<CategorySalesReportItem>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // This is the most complex query. It joins four tables together to link
                // a rental's cost all the way back to the category of the equipment that was rented.
                var query = @"
                    SELECT 
                        cat.Id AS CategoryId,
                        cat.Name AS CategoryName,
                        SUM(ri.RentalCost) AS TotalSales
                    FROM Rentals r
                    JOIN RentalItems ri ON r.Id = ri.RentalId
                    JOIN Equipment eq ON ri.EquipmentId = eq.Id
                    JOIN Categories cat ON eq.CategoryId = cat.Id
                    GROUP BY cat.Id, cat.Name
                    ORDER BY TotalSales DESC;";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reportData.Add(new CategorySalesReportItem
                            {
                                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                TotalSales = reader.GetDecimal(reader.GetOrdinal("TotalSales"))
                            });
                        }
                    }
                }
            }
            return reportData;
        }
    }
}
