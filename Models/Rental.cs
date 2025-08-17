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
        public Customer customer;
        public DateTime RentalDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public DateTime ActualReturnDate { get; set; }
        public double TotalCost { get; set; }
        
        public List<RentalItem> items;

        public Rental()
        {
        }

        public Rental(int id, Customer customer, List<RentalItem> items)
        {
            Id = id;
            this.customer = customer;
            this.items = items;
        }

        public Rental(int id, Customer customer, DateTime rentalDate, DateTime expectedReturnDate) : this(id, customer)
        {
            RentalDate = rentalDate;
            ExpectedReturnDate = expectedReturnDate;
        }

        public Rental(int id, Customer customer)
        {
            Id = id;
            this.customer = customer;
        }

        public double CalculateTotalCost()
        {
            if (items.IsNullOrEmpty()) return 0;

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
