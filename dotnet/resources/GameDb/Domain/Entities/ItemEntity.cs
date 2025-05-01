namespace GameDb.Domain.Entities {
    public class ItemEntity {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte Weight { get; set; }
        public ItemType Type { get; set; }
        public string ImageUrl { get; set; } // TODO
        public byte MaxStackSize { get; set; }
        public bool IsUsable { get; set; }
        public byte? OnlySlot { get; set; } // If not null, the item can only be used in this slot
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