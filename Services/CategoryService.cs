using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Services
{
    internal class CategoryService
    {
        List<Category> categories = new List<Category>();
        public void AddCategory(Category category) { categories.Add(category); }
        public void RemoveCategory(Category category) {categories.Remove(category); }
    }
}
