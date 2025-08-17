using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models.Reports;

namespace VillageRentalManagementSystem.Services
{
    public class ReportService
    {
        private readonly string _connectionString;

        public ReportService()
        {
            _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=VillageRentalDB;Trusted_Connection=True;";
        }

        public async Task<List<DailySalesReportItem>> GetSalesByDateAsync(DateTime salesDate)
        {
            var reportData = new List<DailySalesReportItem>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"
                    SELECT 
                        CAST(r.RentalDate AS DATE) AS SaleDate, 
                        r.Id, 
                        c.FirstName,
                        c.LastName,
                        COUNT(ri.Id) AS NumberOfItems,
                        r.TotalCost
                    FROM Rentals r
                    JOIN Customers c ON r.CustomerId = c.Id
                    JOIN RentalItems ri ON r.Id = ri.RentalId
                    WHERE CAST(r.RentalDate AS DATE) = @SalesDate
                    GROUP BY
                        CAST(r.RentalDate AS DATE),
                        r.Id,
                        c.FirstName,
                        c.LastName,
                        r.TotalCost
                    ORDER BY r.Id;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SalesDate", salesDate.Date);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reportData.Add(new DailySalesReportItem
                            {
                                Date = reader.GetDateTime(reader.GetOrdinal("SaleDate")),
                                RentalId = reader.GetInt32(reader.GetOrdinal("Id")),
                                CustFirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                CustLastName = reader.GetString(reader.GetOrdinal("LastName")),
                                NumberOfItems = reader.GetInt32(reader.GetOrdinal("NumberOfItems")),
                                TotalSales = reader.GetDecimal(reader.GetOrdinal("TotalCost"))
                            });
                        }
                    }
                }
            }
            return reportData;
        }

        public async Task<List<CustomerSalesReportItem>> GetSalesByCustomerAsync()
        {
            var reportData = new List<CustomerSalesReportItem>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
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
                                TotalAmountSpent = reader.IsDBNull(reader.GetOrdinal("TotalAmountSpent")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalAmountSpent"))
                            });
                        }
                    }
                }
            }
            return reportData;
        }

        public async Task<List<CategorySalesReportItem>> GetSalesByCategoryAsync()
        {
            var reportData = new List<CategorySalesReportItem>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
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
