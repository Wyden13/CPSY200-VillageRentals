using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageRentalManagementSystem.Models
{
    public class RentalItem
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public Equipment equipment { get; set; }
        public double rentalCost { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public DateTime? ActualReturnDate { get; set; }
    }
}
