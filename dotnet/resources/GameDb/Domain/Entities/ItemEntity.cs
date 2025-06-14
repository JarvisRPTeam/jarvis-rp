using System.Collections.Generic;
using GameDb.Domain.Models;

namespace GameDb.Domain.Entities {
    public class ItemEntity {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte Weight { get; set; }
        public ItemType Type { get; set; }
        public string ImageUrl { get; set; } // TODO
        public byte MaxStackSize { get; set; }
        public bool IsUsable { get; set; }
        public List<List<InventoryCellModel>> DefaultCells { get; set; } // JSONB default item representation in inventory
    }

    public enum ItemType {
        Weapon,
        Food,
        Drink,
        Heal,
        Clothing,
        Miscellaneous
    }
}