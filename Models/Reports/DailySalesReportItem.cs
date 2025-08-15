using System;

namespace VillageRentalManagementSystem.Models.Reports
{
    public class DailySalesReportItem
    {
        public DateTime Date { get; set; }
        public int RentalId { get; set; }
        public string CustFirstName { get; set; }
        public string CustLastName { get; set; }
        public int NumberOfItems { get; set; }
        public decimal TotalSales { get; set; }
    }
}
