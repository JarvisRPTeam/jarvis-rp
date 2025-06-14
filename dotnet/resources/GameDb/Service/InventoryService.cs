using System.Collections.Generic;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;
using GameDb.Repository;

namespace GameDb.Service {
    public interface IInventoryService {
        Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(long playerId);
        Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(PlayerEntity player);
        Task<DbQueryResult<List<InventoryCellModel>>> GetAllCellsAsync(long playerId);
        Task<DbQueryResult<List<InventoryCellModel>>> GetAllCellsAsync(PlayerEntity player);
        Task<DbQueryResult<InventoryItemModel>> AddItemAsync(long playerId, InventoryItemModel item, bool tryRotate = true);
        Task<DbQueryResult<InventoryItemModel>> AddItemAsync(PlayerEntity player, InventoryItemModel item, bool tryRotate = true);
        Task<DbQueryResult<List<InventoryItemModel>>> AddItemAsync(long playerId, List<InventoryItemModel> items, bool tryRotate = true);
        Task<DbQueryResult<List<InventoryItemModel>>> AddItemAsync(PlayerEntity player, List<InventoryItemModel> items, bool tryRotate = true);
        Task<DbQueryResult<InventoryItemModel>> RemoveItemAsync(long playerId, byte x, byte y);
        Task<DbQueryResult<InventoryItemModel>> RemoveItemAsync(PlayerEntity player, byte x, byte y);
        Task<DbQueryResult<InventoryItemModel>> ChangeItemSlotAsync(long playerId, byte oldX, byte oldY, byte newX, byte newY, bool tryRotate = true);
        Task<DbQueryResult<InventoryItemModel>> ChangeItemSlotAsync(PlayerEntity player, byte oldX, byte oldY, byte newX, byte newY, bool tryRotate = true);
        Task<DbQueryResult<List<InventoryItemModel>>> ClearInventoryAsync(long playerId);
        Task<DbQueryResult<List<InventoryItemModel>>> ClearInventoryAsync(PlayerEntity player);
        static List<List<InventoryCellModel>> RotateItem(List<List<InventoryCellModel>> cells) => InventoryRepository.RotateItem(cells);
    }

    public class InventoryService : IInventoryService {
        public static readonly int DefaultRowCount = 5;
        public static readonly int DefaultColumnCount = 10;

        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(long playerId) =>
            _inventoryRepository.GetAllItemsAsync(playerId);

        public Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(PlayerEntity player) =>
            _inventoryRepository.GetAllItemsAsync(player);

        public Task<DbQueryResult<List<InventoryCellModel>>> GetAllCellsAsync(long playerId) =>
            _inventoryRepository.GetAllCellsAsync(playerId);

        public Task<DbQueryResult<List<InventoryCellModel>>> GetAllCellsAsync(PlayerEntity player) =>
            _inventoryRepository.GetAllCellsAsync(player);

        public async Task<DbQueryResult<InventoryItemModel>> AddItemAsync(long playerId, InventoryItemModel item, bool tryRotate = true) {
            return await _inventoryRepository.AddItemAsync(playerId, item, tryRotate);
        }
        public async Task<DbQueryResult<InventoryItemModel>> AddItemAsync(PlayerEntity player, InventoryItemModel item, bool tryRotate = true) =>
            await AddItemAsync(player.Id, item, tryRotate);

        public async Task<DbQueryResult<List<InventoryItemModel>>> AddItemAsync(long playerId, List<InventoryItemModel> items, bool tryRotate = true) {
            return await _inventoryRepository.AddItemAsync(playerId, items, tryRotate);
        }
        public async Task<DbQueryResult<List<InventoryItemModel>>> AddItemAsync(PlayerEntity player, List<InventoryItemModel> items, bool tryRotate = true) =>
            await AddItemAsync(player.Id, items, tryRotate);

        public async Task<DbQueryResult<InventoryItemModel>> RemoveItemAsync(long playerId, byte x, byte y) {
            return await _inventoryRepository.RemoveItemAsync(playerId, x, y);
        }
        public async Task<DbQueryResult<InventoryItemModel>> RemoveItemAsync(PlayerEntity player, byte x, byte y) =>
            await RemoveItemAsync(player.Id, x, y);

        public async Task<DbQueryResult<InventoryItemModel>> ChangeItemSlotAsync(long playerId, byte oldX, byte oldY, byte newX, byte newY, bool tryRotate = true) {
            return await _inventoryRepository.ChangeItemSlotAsync(playerId, oldX, oldY, newX, newY, tryRotate);
        }
        public async Task<DbQueryResult<InventoryItemModel>> ChangeItemSlotAsync(PlayerEntity player, byte oldX, byte oldY, byte newX, byte newY, bool tryRotate = true) =>
            await ChangeItemSlotAsync(player.Id, oldX, oldY, newX, newY, tryRotate);

        public async Task<DbQueryResult<List<InventoryItemModel>>> ClearInventoryAsync(long playerId) {
            var searchResult = await _inventoryRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType == DbResultType.Error || searchResult.ReturnValue == null) {
                return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Error, "Inventory not found.");
            }
            var inventory = searchResult.ReturnValue;
            inventory.Items.Clear();
            var saveResult = await _inventoryRepository.SaveChangesAsync();
            if (!saveResult) {
                return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Error, "Failed to save inventory.");
            }
            return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Success, "Inventory cleared successfully.", inventory.Items);
        }

        public async Task<DbQueryResult<List<InventoryItemModel>>> ClearInventoryAsync(PlayerEntity player) =>
            await ClearInventoryAsync(player.Id);

        public static List<List<InventoryCellModel>> RotateItem(List<List<InventoryCellModel>> cells) =>
            InventoryRepository.RotateItem(cells);
    }
}