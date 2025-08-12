using System;

namespace VillageRentalManagementSystem.Models.Reports
{
    public class DailySalesReportItem
    {
        public DateTime Date { get; set; }

        public decimal TotalSales { get; set; }
    }
}
