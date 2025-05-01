using System.Text.Json.Serialization;

namespace GameDb.Domain.Entities {
    public class InventoryItem {
        [JsonPropertyName("itemId")]
        public ulong ItemId { get; set; }
        [JsonPropertyName("quantity")]
        public byte Quantity { get; set; }
        [JsonPropertyName("slotIndex")]
        public byte SlotIndex { get; set; }
        [JsonPropertyName("isEquipped")]
        public bool IsEquipped { get; set; }
        [JsonPropertyName("durability")]
        public byte Durability { get; set; }
    }
}