using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Components.Models;

namespace VillageRentalManagementSystem.Models
{
    internal class Equipment
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double DailyRentalCost { get; set; }
        public bool isAvailable { get; set; }
        public Category Category { get; set; }

    }
}
