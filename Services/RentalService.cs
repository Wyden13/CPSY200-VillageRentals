using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Services
{
    internal class RentalService
    {
        List<Rental> rentals = new List<Rental>();

        public void AddRental(Rental rental) { rentals.Add(rental);}
        public void RemoveRental(Rental rental) { rentals.Remove(rental);}
    }
}
