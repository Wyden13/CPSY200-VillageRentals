using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageRentalManagementSystem.Models
{
    public class RentalItem
    {
        public Equipment equipment;
        public DateTime rentalDate;
        public DateTime returnDAte;
        public double rentalCost;

        public RentalItem()
        {
        }

        public RentalItem(Equipment equipment, DateTime rentalDate, DateTime returnDAte, double rentalCost)
        {
            this.equipment = equipment;
            this.rentalDate = rentalDate;
            this.returnDAte = returnDAte;
            this.rentalCost = rentalCost;
        }
    }
}
