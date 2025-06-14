using System.Collections.Generic;
using GameDb.Domain.Models;

namespace GameDb.Domain.Entities {
    public class InventoryEntity {
        public long PlayerId { get; set; } // Foreign key to Player, Primary key
        public List<InventoryItemModel> Items { get; set; } = new List<InventoryItemModel>(); // JSONB all items
        public List<List<InventoryCellModel>> Cells { get; set; } = new List<List<InventoryCellModel>>(); // JSONB cells of the items
        public byte TotalWeight { get; set; }
        public byte MaxWeight { get; set; } 

        // Navigation properties
        public virtual PlayerEntity Player { get; set; }
    }
}