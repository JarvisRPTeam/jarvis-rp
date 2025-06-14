#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;

namespace GameDb.Repository {
    public interface IInventoryRepository: IGameDbRepository<InventoryEntity> {
        Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(long playerId);
        Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(PlayerEntity player);
        Task<DbQueryResult<List<InventoryCellModel>>> GetAllCellsAsync(long playerId);
        Task<DbQueryResult<List<InventoryCellModel>>> GetAllCellsAsync(PlayerEntity player);
        Task<DbQueryResult<InventoryItemModel>> AddItemAsync(long playerId, InventoryItemModel item, bool tryRotate = true);
        Task<DbQueryResult<List<InventoryItemModel>>> AddItemAsync(long playerId, List<InventoryItemModel> items, bool tryRotate = true);
        Task<DbQueryResult<InventoryItemModel>> RemoveItemAsync(long playerId, byte x, byte y);
        Task<DbQueryResult<InventoryItemModel>> ChangeItemSlotAsync(long playerId, byte oldX, byte oldY, byte newX, byte newY, bool tryRotate = false);
    }

    public class InventoryRepository : GameDbRepository<InventoryEntity>, IInventoryRepository {

        public InventoryRepository(GameDbContext context): base(context) {
        }

        public async Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(long playerId) {
            var searchResult = await GetByIdAsync(playerId);
            if (searchResult.ResultType == DbResultType.Error || searchResult.ReturnValue == null) {
                return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Error, "Inventory not found.");
            }
            return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Success, "Inventory retrieved successfully.", searchResult.ReturnValue.Items);
        }
        public async Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(PlayerEntity player) => await GetAllItemsAsync(player.Id);

        public async Task<DbQueryResult<List<InventoryCellModel>>> GetAllCellsAsync(long playerId) {
            var searchResult = await GetByIdAsync(playerId);
            if (searchResult.ResultType == DbResultType.Error || searchResult.ReturnValue == null) {
                return new DbQueryResult<List<InventoryCellModel>>(DbResultType.Error, "Inventory not found.");
            }
            var flatCells = searchResult.ReturnValue.Cells.SelectMany(row => row).ToList();
            return new DbQueryResult<List<InventoryCellModel>>(DbResultType.Success, "Cells retrieved successfully.", flatCells);
        }
        public async Task<DbQueryResult<List<InventoryCellModel>>> GetAllCellsAsync(PlayerEntity player) => await GetAllCellsAsync(player.Id);

        private ItemEntity? GetItemEntity(long itemId) => _context.Items.FirstOrDefault(i => i.Id == itemId);

        public static List<List<List<InventoryCellModel>>> GetAllRotations(List<List<InventoryCellModel>> cells) {
            var rotations = new List<List<List<InventoryCellModel>>>();
            var current = cells;
            for (int i = 0; i < 4; i++) {
                rotations.Add(current);
                current = RotateItem(current);
            }
            return rotations;
        }

        public static List<List<InventoryCellModel>> RotateItem(List<List<InventoryCellModel>> cells) {
            int rows = cells.Count, cols = cells[0].Count;
            var rotated = new List<List<InventoryCellModel>>();
            for (int y = 0; y < cols; y++) {
                var newRow = new List<InventoryCellModel>();
                for (int x = rows - 1; x >= 0; x--) {
                    var cell = new InventoryCellModel {
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

        public static bool CanPlace(List<List<InventoryCellModel>> grid, List<List<InventoryCellModel>> itemCells, int startX, int startY) {
            int itemRows = itemCells.Count;
            int itemCols = itemCells[0].Count;
            int gridRows = grid.Count;
            int gridCols = grid[0].Count;
            if (startX + itemRows > gridRows || startY + itemCols > gridCols) return false;
            for (int x = 0; x < itemRows; x++) {
                for (int y = 0; y < itemCols; y++) {
                    if (itemCells[x][y].InventoryItem != null) continue; // skip empty cell
                    if (grid[startX + x][startY + y].InventoryItem != null) return false;
                }
            }
            return true;
        }

        public static void PlaceItem(List<List<InventoryCellModel>> grid, List<List<InventoryCellModel>> itemCells, InventoryItemModel item, int startX, int startY) {
            int itemRows = itemCells.Count;
            int itemCols = itemCells[0].Count;
            for (int x = 0; x < itemRows; x++) {
                for (int y = 0; y < itemCols; y++) {
                    if (itemCells[x][y].InventoryItem != null) {
                        grid[startX + x][startY + y].InventoryItem = item;
                    }
                }
            }
        }

        public static void RemoveItemFromGrid(List<List<InventoryCellModel>> grid, InventoryItemModel item) {
            foreach (var row in grid) {
                foreach (var cell in row) {
                    if (cell.InventoryItem == item) cell.InventoryItem = null;
                }
            }
        }

        public static (int, int, List<List<InventoryCellModel>>)? FindPlacement(List<List<InventoryCellModel>> grid, List<List<InventoryCellModel>> itemCells, bool tryRotate) {
            var rotations = tryRotate ? GetAllRotations(itemCells) : new List<List<List<InventoryCellModel>>> { itemCells };
            foreach (var shape in rotations) {
                int rows = grid.Count, cols = grid[0].Count;
                int itemRows = shape.Count, itemCols = shape[0].Count;
                for (int x = 0; x <= rows - itemRows; x++) {
                    for (int y = 0; y <= cols - itemCols; y++) {
                        if (CanPlace(grid, shape, x, y)) return (x, y, shape);
                    }
                }
            }
            return null;
        }

        public async Task<DbQueryResult<InventoryItemModel>> AddItemAsync(long playerId, InventoryItemModel item, bool tryRotate = true) {
            var searchResult = await GetByIdAsync(playerId);
            if (searchResult.ResultType == DbResultType.Error || searchResult.ReturnValue == null) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Inventory not found.");
            }
            var inventory = searchResult.ReturnValue;
            var itemEntity = GetItemEntity(item.ItemId);
            if (itemEntity == null) return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Item entity not found.");
            var itemCells = itemEntity.DefaultCells;
            var placement = FindPlacement(inventory.Cells, itemCells, tryRotate);
            if (placement == null) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Warning, "No space to add item.");
            }
            var (x, y, shape) = placement.Value;
            PlaceItem(inventory.Cells, shape, item, x, y);
            var saveResult = await SaveChangesAsync();
            if (!saveResult) {
                RemoveItemFromGrid(inventory.Cells, item);
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Failed to save inventory.");
            }
            return new DbQueryResult<InventoryItemModel>(DbResultType.Success, "Item added successfully.", item);
        }

        public async Task<DbQueryResult<List<InventoryItemModel>>> AddItemAsync(long playerId, List<InventoryItemModel> items, bool tryRotate = true) {
            var added = new List<InventoryItemModel>();
            foreach (var item in items) {
                var result = await AddItemAsync(playerId, item, tryRotate);
                if (result.ResultType != DbResultType.Success) {
                    var searchResult = await GetByIdAsync(playerId);
                    if (searchResult.ReturnValue != null) {
                        foreach (var addedItem in added) RemoveItemFromGrid(searchResult.ReturnValue.Cells, addedItem);
                        await SaveChangesAsync();
                    }
                    return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Warning, $"Failed to add all items: {result.Message}");
                }
                added.Add(item);
            }
            return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Success, "Items added successfully.", added);
        }

        public async Task<DbQueryResult<InventoryItemModel>> RemoveItemAsync(long playerId, byte x, byte y) {
            var searchResult = await GetByIdAsync(playerId);
            if (searchResult.ResultType == DbResultType.Error || searchResult.ReturnValue == null) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Inventory not found.");
            }
            var inventory = searchResult.ReturnValue;
            if (x >= inventory.Cells.Count || y >= inventory.Cells[0].Count) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Invalid cell coordinates.");
            }
            var item = inventory.Cells[x][y].InventoryItem;
            if (item == null) return new DbQueryResult<InventoryItemModel>(DbResultType.Warning, "No item in specified cell.");
            RemoveItemFromGrid(inventory.Cells, item);
            var saveResult = await SaveChangesAsync();
            if (!saveResult) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Failed to save inventory.");
            }
            return new DbQueryResult<InventoryItemModel>(DbResultType.Success, "Item removed successfully.", item);
        }

        public async Task<DbQueryResult<InventoryItemModel>> ChangeItemSlotAsync(long playerId, byte oldX, byte oldY, byte newX, byte newY, bool tryRotate = false) {
            var searchResult = await GetByIdAsync(playerId);
            if (searchResult.ResultType == DbResultType.Error || searchResult.ReturnValue == null) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Inventory not found.");
            }
            var inventory = searchResult.ReturnValue;
            if (oldX >= inventory.Cells.Count || oldY >= inventory.Cells[0].Count) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Invalid old cell coordinates.");
            }
            var item = inventory.Cells[oldX][oldY].InventoryItem;
            if (item == null) return new DbQueryResult<InventoryItemModel>(DbResultType.Warning, "No item in specified cell.");
            var itemEntity = GetItemEntity(item.ItemId);
            if (itemEntity == null) return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Item entity not found.");
            var itemCells = itemEntity.DefaultCells;
            RemoveItemFromGrid(inventory.Cells, item);
            var placement = FindPlacement(inventory.Cells, itemCells, tryRotate);
            bool placed = false;
            if (placement != null) {
                var (x, y, shape) = placement.Value;
                if (x == newX && y == newY) {
                    PlaceItem(inventory.Cells, shape, item, x, y);
                    placed = true;
                }
            }
            if (!placed) {
                PlaceItem(inventory.Cells, itemCells, item, oldX, oldY);
                return new DbQueryResult<InventoryItemModel>(DbResultType.Warning, "Cannot move item to new position.");
            }
            var saveResult = await SaveChangesAsync();
            if (!saveResult) {
                RemoveItemFromGrid(inventory.Cells, item);
                PlaceItem(inventory.Cells, itemCells, item, oldX, oldY);
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Failed to save inventory.");
            }
            return new DbQueryResult<InventoryItemModel>(DbResultType.Success, "Item slot changed successfully.", item);
        }
    }
}
