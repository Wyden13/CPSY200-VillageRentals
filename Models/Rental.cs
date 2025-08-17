using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public Customer customer { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public double TotalCost { get; set; }
        public List<RentalItem> items { get; set; } = new List<RentalItem>();

        public double CalculateTotalCost()
        {
            return items?.Sum(item => item.rentalCost) ?? 0;
        }
    }
}
