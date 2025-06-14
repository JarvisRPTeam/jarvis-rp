#nullable enable
namespace GameDb.Domain.Models
{
    public class InventoryCellModel
    {
        public byte X { get; set; }
        public byte Y { get; set; }

        public InventoryItemModel? InventoryItem { get; set; }
    }
}