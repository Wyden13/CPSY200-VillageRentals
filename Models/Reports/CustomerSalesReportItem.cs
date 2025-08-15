
namespace VillageRentalManagementSystem.Models.Reports
{
    public class CustomerSalesReportItem
    {
        public int CustomerId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public decimal TotalAmountSpent { get; set; }
    }
}
