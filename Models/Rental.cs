using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Components.Models;

namespace VillageRentalManagementSystem.Models
{
    internal class Rental
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
    }
}
