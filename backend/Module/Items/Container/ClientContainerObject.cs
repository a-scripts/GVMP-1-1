using System;
using System.Collections.Generic;
using System.Text;
using VMP_CNR.Module.Items;

namespace VMP_CNR.Module.Items
{
    public class PlayerContainerObject
    {
        public int Id { get; set; }
        public int MaxWeight { get; set; }
        public int MaxSlots { get; set; }
        public string Name { get; set; }
        public int Money { get; set; }
        public int Blackmoney { get; set; }

        public bool PlayerInventory { get; set; }
        public List<PlayerContainerSlotObject> Slots { get; set; }

        public PlayerContainerObject(int maxWeight, int maxSlots)
        {
            MaxWeight = maxWeight;
            MaxSlots = maxSlots;
            Slots = new List<PlayerContainerSlotObject>();
            PlayerInventory = false;
        }

    }

    public class PlayerContainerSlotObject
    {
        public int Id { get; set; }
        public int Slot { get; set; }
        public Dictionary<string, dynamic> Data { get; set; }
        public int Amount { get; set; }
        public int Weight { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }

        public PlayerContainerSlotObject(int id, int slot, ItemModel model, Dictionary<string, dynamic> data, int amount)
        {
            Id = id;
            Slot = slot;
            Name = model.Name;
            Weight = model.Weight;
            ImagePath = model.ImagePath;
            Data = data;
            Amount = amount;
        }
    }
}
