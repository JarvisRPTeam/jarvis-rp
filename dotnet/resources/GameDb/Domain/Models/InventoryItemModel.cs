using System.Text.Json.Serialization;

namespace GameDb.Domain.Models
{
    public class InventoryItemModel
    {
        [JsonPropertyName("itemId")]
        public long ItemId { get; set; }
        [JsonPropertyName("quantity")]
        public byte Quantity { get; set; }
        [JsonPropertyName("isEquipped")]
        public bool IsEquipped { get; set; }
        [JsonPropertyName("durability")]
        public byte Durability { get; set; }
    }
}