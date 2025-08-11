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
        public Customer customer;
        
        List<RentalItem> items;

        public Rental()
        {
        }

        public Rental(int id, Customer customer, List<RentalItem> items)
        {
            Id = id;
            this.customer = customer;
            this.items = items;
        }

        public double CalculateTotalCost()
        {
            double totalCost = 0;
            foreach (RentalItem item in items)
            {
                totalCost += item.rentalCost;
            }
            return totalCost;
        }

        public void AddRentalItem(RentalItem item) { items.Add(item); }
    }
}
