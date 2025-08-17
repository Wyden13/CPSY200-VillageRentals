using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageRentalManagementSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsBanned { get; set; }
        public double DiscountRate { get; set; }
        List<Rental> rentalHistory;

        public Customer(int id, string firstName, string lastName, string phoneNumber, string email, bool isBanned, double discountRate, List<Rental> rentalHistory) : this(id, firstName, lastName, phoneNumber, email, isBanned, discountRate)
        {
            this.rentalHistory = rentalHistory;
        }

        public Customer(int id, string firstName, string lastName, string phoneNumber, string email, bool isBanned, double discountRate)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
            IsBanned = isBanned;
            DiscountRate = discountRate;
        }

        public Customer(int id, string firstName, string lastname, string email)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastname;
            this.Email = email;
        }

        public Customer()
        {
        }
    }
}
