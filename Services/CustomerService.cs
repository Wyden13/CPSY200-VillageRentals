using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Services
{
    internal class CustomerService
    {
        List<Customer> customers = new List<Customer>();

        public void AddCustomer(Customer customer) { customers.Add(customer); }
        public void RemoveCustomer(Customer customer) {customers.Remove(customer); }

    }
}
