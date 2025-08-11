using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageRentalManagementSystem.Models;

namespace VillageRentalManagementSystem.Services
{
    internal class EquipmentService
    {
        List<Equipment> equipmentList = new List<Equipment>();

        public void AddEquipment(Equipment equipment) { equipmentList.Add(equipment); }

        public void RemoveEquipment(Equipment equipment) { equipmentList.Remove(equipment); }

        public bool CheckEquipmentAvailability(Equipment equipment)
        {
            foreach (Equipment equipmentItem in equipmentList) {
                if (equipmentItem.Equals(equipment))
                {
                    return equipment.IsAvailable;
                }
            }
            return false;
        }

    }
}
