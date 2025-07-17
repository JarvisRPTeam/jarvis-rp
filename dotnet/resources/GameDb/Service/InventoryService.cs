using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;
using GameDb.Repository;

namespace GameDb.Service {
    public interface IInventoryService
    {
        Task<InventoryEntity> CreateInventoryAsync(
            int rowCount = InventoryService.DefaultRowCount,
            int columnCount = InventoryService.DefaultColumnCount,
            byte maxWeight = InventoryService.DefaultMaxWeight
        );
        Task<bool> AddItemAsync(InventoryEntity inventory, InventoryItemModel item);
        Task<bool> AddItemsAsync(InventoryEntity inventory, IEnumerable<InventoryItemModel> items);
        Task<bool> PlaceItemAsync(InventoryEntity inventory, InventoryItemModel item, byte x, byte y);
        Task<bool> RemoveItemAsync(InventoryEntity inventory, byte x, byte y);
        Task<bool> ChangeItemSlotAsync(InventoryEntity inventory, byte oldX, byte oldY, byte newX, byte newY);
        Task<bool> ClearInventoryAsync(InventoryEntity inventory);
        Task<ItemEntity> GetItemByIdAsync(long itemId);
    }

    public class InventoryService : IInventoryService
    {
        public const int DefaultRowCount = 5;
        public const int DefaultColumnCount = 10;
        public const byte DefaultMaxWeight = 100;

        private readonly IInventoryRepository _inventoryRepository;
        private readonly IItemRepository _itemRepository;
        private readonly GameDbContext _context;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IItemRepository itemRepository,
            GameDbContext context
        )
        {
            _itemRepository = itemRepository;
            _inventoryRepository = inventoryRepository;
            _context = context;
        }

        // Item
        public async Task<ItemEntity> GetItemByIdAsync(long itemId)
        {
            if (itemId <= 0)
            {
                Console.WriteLine("Invalid item ID.");
                return null;
            }
            var item = await _itemRepository.GetByIdAsync(itemId);
            if (item.ResultType != DbResultType.Success)
            {
                Console.WriteLine($"Error retrieving item: {item.Message}");
                return null;
            }
            if (item.ReturnValue == null)
            {
                Console.WriteLine("Item not found.");
            }
            return item.ReturnValue;
        }

        // Inventory
        public async Task<InventoryEntity> CreateInventoryAsync(
            int rowCount = DefaultRowCount,
            int columnCount = DefaultColumnCount,
            byte maxWeight = DefaultMaxWeight
        )
        {
            List<List<InventoryCellModel>> cells = new List<List<InventoryCellModel>>();
            for (int i = 0; i < rowCount; i++)
            {
                List<InventoryCellModel> row = new List<InventoryCellModel>();
                for (int j = 0; j < columnCount; j++)
                {
                    row.Add(new InventoryCellModel { X = (byte)i, Y = (byte)j, InventoryItem = null });
                }
                cells.Add(row);
            }

            var inventory = new InventoryEntity
            {
                Cells = cells,
                MaxWeight = maxWeight,
                TotalWeight = 0
            };

            var addResult = await _inventoryRepository.AddAsync(inventory);
            if (addResult.ResultType != DbResultType.Success)
            {
                Console.WriteLine($"Error creating inventory: {addResult.Message}");
                return null;
            }

            var saveResult = await _inventoryRepository.SaveChangesAsync();
            if (!saveResult)
            {
                Console.WriteLine("Failed to save inventory to database.");
                return null;
            }
            return inventory;
        }

        public static IEnumerable<InventoryCellModel> GetAllCells(InventoryEntity inventory)
        {
            if (inventory == null || inventory.Cells == null || inventory.Cells.Count == 0)
            {
                Console.WriteLine("Inventory or cells are null or empty.");
                return new List<InventoryCellModel>();
            }
            var allCells = new List<InventoryCellModel>();
            foreach (var row in inventory.Cells)
            {
                allCells.AddRange(row);
            }
            return allCells;
        }

        public async Task<bool> AddItemAsync(InventoryEntity inventory, InventoryItemModel item)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await AddItemTransactionlessAsync(inventory, item);
                if (!result)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                var saveResult = await _inventoryRepository.SaveChangesAsync();
                if (!saveResult)
                {
                    Console.WriteLine("Failed to save inventory after adding item.");
                    await transaction.RollbackAsync();
                    return false;
                }
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> AddItemsAsync(InventoryEntity inventory, IEnumerable<InventoryItemModel> items)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in items)
                {
                    var result = await AddItemTransactionlessAsync(inventory, item);
                    if (!result)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
                var saveResult = await _inventoryRepository.SaveChangesAsync();
                if (!saveResult)
                {
                    Console.WriteLine("Failed to save inventory after adding items.");
                    await transaction.RollbackAsync();
                    return false;
                }
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> PlaceItemAsync(InventoryEntity inventory, InventoryItemModel item, byte x, byte y)
        {
            if (inventory == null)
            {
                Console.WriteLine("Inventory is null.");
                return false;
            }
            if (x >= inventory.Cells.Count || y >= inventory.Cells[0].Count)
            {
                Console.WriteLine("Invalid cell coordinates.");
                return false;
            }
            var itemEntity = await GetItemByIdAsync(item.ItemId);
            if (itemEntity == null)
            {
                Console.WriteLine("Item entity not found.");
                return false;
            }
            var itemCells = itemEntity.DefaultCells;
            if (!CanPlace(inventory.Cells, itemCells, x, y))
            {
                Console.WriteLine("Cannot place item in specified cell.");
                return false;
            }
            PlaceItemOnGrid(inventory.Cells, itemCells, item, x, y);
            var saveResult = await _inventoryRepository.SaveChangesAsync();
            if (!saveResult)
            {
                RemoveItemFromGrid(inventory.Cells, item);
                Console.WriteLine("Failed to save inventory after placing item.");
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveItemAsync(InventoryEntity inventory, byte x, byte y)
        {
            if (inventory == null)
            {
                Console.WriteLine("Inventory is null.");
                return false;
            }
            if (x >= inventory.Cells.Count || y >= inventory.Cells[0].Count)
            {
                Console.WriteLine("Invalid cell coordinates.");
                return false;
            }
            var item = inventory.Cells[x][y].InventoryItem;
            if (item == null)
            {
                Console.WriteLine("No item in specified cell.");
                return false;
            }
            RemoveItemFromGrid(inventory.Cells, item);
            var saveResult = await _inventoryRepository.SaveChangesAsync();
            if (!saveResult)
            {
                Console.WriteLine("Failed to save inventory.");
                return false;
            }
            return true;
        }

        public async Task<bool> ChangeItemSlotAsync(InventoryEntity inventory, byte oldX, byte oldY, byte newX, byte newY)
        {
            if (inventory == null)
            {
                Console.WriteLine("Inventory is null.");
                return false;
            }
            if (oldX >= inventory.Cells.Count || oldY >= inventory.Cells[0].Count)
            {
                Console.WriteLine("Invalid old cell coordinates.");
                return false;
            }
            var item = inventory.Cells[oldX][oldY].InventoryItem;
            if (item == null)
            {
                Console.WriteLine("No item in specified cell.");
                return false;
            }
            var itemEntity = await GetItemByIdAsync(item.ItemId);
            if (itemEntity == null)
            {
                Console.WriteLine("Item entity not found.");
                return false;
            }
            var itemCells = itemEntity.DefaultCells;
            RemoveItemFromGrid(inventory.Cells, item);
            var placement = FindPlacement(inventory.Cells, itemCells, false);
            bool placed = false;
            if (placement != null)
            {
                var (x, y, shape) = placement.Value;
                if (x == newX && y == newY)
                {
                    PlaceItemOnGrid(inventory.Cells, shape, item, x, y);
                    placed = true;
                }
            }
            if (!placed)
            {
                PlaceItemOnGrid(inventory.Cells, itemCells, item, oldX, oldY);
                Console.WriteLine("Cannot move item to new position.");
                return false;
            }
            var saveResult = await _inventoryRepository.SaveChangesAsync();
            if (!saveResult)
            {
                RemoveItemFromGrid(inventory.Cells, item);
                PlaceItemOnGrid(inventory.Cells, itemCells, item, oldX, oldY);
                Console.WriteLine("Failed to save inventory after changing item slot.");
                return false;
            }
            return true;
        }

        public async Task<bool> ClearInventoryAsync(InventoryEntity inventory)
        {
            if (inventory == null)
            {
                Console.WriteLine("Inventory is null.");
                return false;
            }
            inventory.Items.Clear();
            foreach (var row in inventory.Cells)
            {
                foreach (var cell in row)
                {
                    cell.InventoryItem = null;
                }
            }
            var saveResult = await _inventoryRepository.SaveChangesAsync();
            if (!saveResult)
            {
                Console.WriteLine("Failed to save inventory after clearing.");
                return false;
            }
            return true;
        }

        public static List<List<List<InventoryCellModel>>> GetAllRotations(List<List<InventoryCellModel>> cells)
        {
            var rotations = new List<List<List<InventoryCellModel>>>();
            var current = cells;
            for (int i = 0; i < 4; i++)
            {
                rotations.Add(current);
                current = RotateItem(current);
            }
            return rotations;
        }

        public static List<List<InventoryCellModel>> RotateItem(List<List<InventoryCellModel>> cells)
        {
            if (cells.Count == 0 || cells[0].Count == 0)
            {
                throw new ArgumentException("Item cells cannot be empty.");
            }
            int rows = cells.Count, cols = cells[0].Count;
            var rotated = new List<List<InventoryCellModel>>();
            for (int y = 0; y < cols; y++)
            {
                var newRow = new List<InventoryCellModel>();
                for (int x = rows - 1; x >= 0; x--)
                {
                    var cell = new InventoryCellModel
                    {
                        X = (byte)y,
                        Y = (byte)(rows - 1 - x),
                        InventoryItem = null
                    };
                    if (cells[x][y].InventoryItem != null) cell.InventoryItem = cells[x][y].InventoryItem;
                    newRow.Add(cell);
                }
                rotated.Add(newRow);
            }
            return rotated;
        }

        public static bool CanPlace(List<List<InventoryCellModel>> grid, List<List<InventoryCellModel>> itemCells, int startX, int startY)
        {
            if (itemCells.Count == 0 || itemCells[0].Count == 0)
            {
                throw new ArgumentException("Item cells cannot be empty.");
            }
            if (grid.Count == 0 || grid[0].Count == 0)
            {
                throw new ArgumentException("Grid cannot be empty.");
            }
            int itemRows = itemCells.Count;
            int itemCols = itemCells[0].Count;
            int gridRows = grid.Count;
            int gridCols = grid[0].Count;
            if (startX + itemRows > gridRows || startY + itemCols > gridCols) return false;
            for (int x = 0; x < itemRows; x++)
            {
                for (int y = 0; y < itemCols; y++)
                {
                    if (itemCells[x][y].InventoryItem == null) continue; // skip empty cell
                    if (grid[startX + x][startY + y].InventoryItem != null) return false;
                }
            }
            return true;
        }

        public static void PlaceItemOnGrid(List<List<InventoryCellModel>> grid, List<List<InventoryCellModel>> itemCells, InventoryItemModel item, int startX, int startY)
        {
            if (itemCells.Count == 0 || itemCells[0].Count == 0)
            {
                throw new ArgumentException("Item cells cannot be empty.");
            }
            if (grid.Count == 0 || grid[0].Count == 0)
            {
                throw new ArgumentException("Grid cannot be empty.");
            }
            int itemRows = itemCells.Count;
            int itemCols = itemCells[0].Count;
            for (int x = 0; x < itemRows; x++)
            {
                for (int y = 0; y < itemCols; y++)
                {
                    if (itemCells[x][y].InventoryItem != null)
                    {
                        grid[startX + x][startY + y].InventoryItem = item;
                    }
                }
            }
        }

        public static void RemoveItemFromGrid(List<List<InventoryCellModel>> grid, InventoryItemModel item)
        {
            foreach (var row in grid)
            {
                foreach (var cell in row)
                {
                    if (cell.InventoryItem == item) cell.InventoryItem = null;
                }
            }
        }

        public static (int, int, List<List<InventoryCellModel>>)? FindPlacement(List<List<InventoryCellModel>> grid, List<List<InventoryCellModel>> itemCells, bool tryRotate)
        {
            if (grid.Count == 0 || grid[0].Count == 0)
            {
                throw new ArgumentException("Grid cannot be empty.");
            }
            var rotations = tryRotate ? GetAllRotations(itemCells) : new List<List<List<InventoryCellModel>>> { itemCells };
            foreach (var shape in rotations)
            {
                int rows = grid.Count, cols = grid[0].Count;
                int itemRows = shape.Count, itemCols = shape[0].Count;
                for (int x = 0; x <= rows - itemRows; x++)
                {
                    for (int y = 0; y <= cols - itemCols; y++)
                    {
                        if (CanPlace(grid, shape, x, y)) return (x, y, shape);
                    }
                }
            }
            return null;
        }
        
        private async Task<bool> AddItemTransactionlessAsync(InventoryEntity inventory, InventoryItemModel item)
        {
            if (inventory == null)
            {
                Console.WriteLine("Inventory is null.");
                return false;
            }
            if (item == null)
            {
                Console.WriteLine("Item is null.");
                return false;
            }
            try
            {
                var itemEntity = await GetItemByIdAsync(item.ItemId);
                if (itemEntity == null)
                {
                    Console.WriteLine("Item entity not found.");
                    return false;
                }
                var itemCells = itemEntity.DefaultCells;
                var allCells = GetAllCells(inventory);

                var existingStacks = allCells
                    .Where(c => c.InventoryItem != null
                        && c.InventoryItem.ItemId == item.ItemId
                    )
                    .Select(c => c.InventoryItem)
                    .Distinct()
                    .ToList();

                int quantityToAdd = item.Quantity;
                bool isFullDurability = item.Durability >= 100;

                foreach (var stack in existingStacks)
                {
                    if (quantityToAdd <= 0 || (quantityToAdd == 1 && !isFullDurability))
                    {
                        break;
                    }
                    if (stack.Quantity > itemEntity.MaxStackSize)
                        continue;

                    int space = itemEntity.MaxStackSize - stack.Quantity;
                    int add = Math.Min(space, quantityToAdd);
                    stack.Quantity += (byte)add;
                    quantityToAdd -= add;
                }

                if (!isFullDurability)
                {
                    var singleItem = new InventoryItemModel
                    {
                        ItemId = item.ItemId,
                        Quantity = 1,
                        IsEquipped = item.IsEquipped,
                        Durability = item.Durability
                    };
                    var placement = FindPlacement(inventory.Cells, itemCells, true);
                    if (placement == null)
                    {
                        Console.WriteLine("No space to place item with durability < 100.");
                        return false;
                    }
                    var (x, y, shape) = placement.Value;
                    PlaceItemOnGrid(inventory.Cells, shape, singleItem, x, y);
                    quantityToAdd -= 1;
                }

                while (quantityToAdd > 0)
                {
                    int stackSize = Math.Min(itemEntity.MaxStackSize, quantityToAdd);
                    var newStack = new InventoryItemModel
                    {
                        ItemId = item.ItemId,
                        Quantity = (byte)stackSize,
                        IsEquipped = item.IsEquipped,
                        Durability = item.Durability
                    };
                    var placement = FindPlacement(inventory.Cells, itemCells, true);
                    if (placement == null)
                    {
                        Console.WriteLine("No space to place remaining stack.");
                        return false;
                    }
                    var (x, y, shape) = placement.Value;
                    PlaceItemOnGrid(inventory.Cells, shape, newStack, x, y);
                    quantityToAdd -= stackSize;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while adding item: {ex.Message}");
                return false;
            }
        }
    }
}