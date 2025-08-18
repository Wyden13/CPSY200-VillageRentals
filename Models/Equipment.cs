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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double DailyRentalCost { get; set; }
        public bool IsAvailable { get; set; }
        public Category Category { get; set; }
        public bool IsDeleted { get; set; }

        public Equipment()
        {
            IsAvailable = true;
            Category = new Category();
        }
    }
}
