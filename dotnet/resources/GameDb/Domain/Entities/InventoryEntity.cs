using System.Collections.Generic;

namespace GameDb.Domain.Entities {
    public class InventoryEntity {
        public ulong PlayerId { get; set; } // Foreign key to Player, Primary key
        public List<InventoryItem> Items { get; set; } = new List<InventoryItem>(); // JSONB array
        public byte TotalWeight { get; set; }
        public byte MaxWeight { get; set; } 

        // Navigation properties
        public virtual PlayerEntity Player { get; set; }
    }
}