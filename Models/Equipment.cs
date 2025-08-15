using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Models
{
    public class Equipment
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double DailyRentalCost { get; set; }
        public bool IsAvailable {  get; set; }

        public Category Category;
        //public int CategoryId { get;set; }
        public Equipment(int id, string name, string description, double dailyRentalCost, bool isAvailable, Category category) : this(id, name, description, dailyRentalCost, isAvailable)
        {
            Category = category;
        }

        public Equipment(int id, string name, string description, double dailyRentalCost, bool isAvailable)
        {
            Id = id;
            Name = name;
            Description = description;
            DailyRentalCost = dailyRentalCost;
            
        }

        public Equipment()
        {
            IsAvailable = true;
        }

    }
}
