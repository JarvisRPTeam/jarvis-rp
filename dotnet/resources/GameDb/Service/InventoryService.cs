using System.Collections.Generic;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;
using GameDb.Repository;

namespace GameDb.Service {
    public interface IInventoryService {
        Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(long playerId);
        Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(PlayerEntity player);
        Task<DbQueryResult<InventoryItemModel>> AddItemAsync(long playerId, InventoryItemModel item);
        Task<DbQueryResult<InventoryItemModel>> AddItemAsync(PlayerEntity player, InventoryItemModel item);
        Task<DbQueryResult<List<InventoryItemModel>>> AddItemAsync(long playerId, List<InventoryItemModel> items);
        Task<DbQueryResult<List<InventoryItemModel>>> AddItemAsync(PlayerEntity player, List<InventoryItemModel> items);
        Task<DbQueryResult<InventoryItemModel>> RemoveItemAsync(long playerId, byte slotIndex);
        Task<DbQueryResult<InventoryItemModel>> RemoveItemAsync(PlayerEntity player, byte slotIndex);
        Task<DbQueryResult<InventoryItemModel>> ChangeItemSlotAsync(long playerId, byte oldSlotIndex, byte newSlotIndex);
        Task<DbQueryResult<InventoryItemModel>> ChangeItemSlotAsync(PlayerEntity player, byte oldSlotIndex, byte newSlotIndex);
        Task<DbQueryResult<List<InventoryItemModel>>> ClearInventoryAsync(long playerId);
        Task<DbQueryResult<List<InventoryItemModel>>> ClearInventoryAsync(PlayerEntity player);
    }

    public class InventoryService : IInventoryService {
        private readonly IGameDbRepository<InventoryEntity> _inventoryRepository;
        private readonly GameDbContext _context;

        public InventoryService(IGameDbRepository<InventoryEntity> inventoryRepository, GameDbContext context) {
            _inventoryRepository = inventoryRepository;
            _context = context;
        }

        public async Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(long playerId) {
            var searchResult = await _inventoryRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType == DbResultType.Error || searchResult.ReturnValue == null) {
                return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Error, "Inventory not found.");
            }

            var inventory = searchResult.ReturnValue;
            return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Success, "Inventory retrieved successfully.", inventory.Items);
        }

        public async Task<DbQueryResult<List<InventoryItemModel>>> GetAllItemsAsync(PlayerEntity player) {
            return await GetAllItemsAsync(player.Id);
        }

        public async Task<DbQueryResult<InventoryItemModel>> AddItemAsync(long playerId, InventoryItemModel item) {
            var searchResult = await _inventoryRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType == DbResultType.Error || searchResult.ReturnValue == null) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Inventory not found.");
            }

            var inventory = searchResult.ReturnValue;
            inventory.Items.Add(item);
            var saveResult = await _inventoryRepository.SaveChangesAsync();
            if (!saveResult) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Failed to save inventory.");
            }

            return new DbQueryResult<InventoryItemModel>(DbResultType.Success, "Item added successfully.", item);
        }

        public async Task<DbQueryResult<InventoryItemModel>> AddItemAsync(PlayerEntity player, InventoryItemModel item) {
            return await AddItemAsync(player.Id, item);
        }

        public async Task<DbQueryResult<List<InventoryItemModel>>> AddItemAsync(long playerId, List<InventoryItemModel> items) {
            var searchResult = await _inventoryRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType == DbResultType.Error || searchResult.ReturnValue == null) {
                return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Error, "Inventory not found.");
            }

            var inventory = searchResult.ReturnValue;
            inventory.Items.AddRange(items);
            var saveResult = await _inventoryRepository.SaveChangesAsync();
            if (!saveResult) {
                return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Error, "Failed to save inventory.");
            }

            return new DbQueryResult<List<InventoryItemModel>>(DbResultType.Success, "Items added successfully.", items);
        }

        public async Task<DbQueryResult<List<InventoryItemModel>>> AddItemAsync(PlayerEntity player, List<InventoryItemModel> items) {
            return await AddItemAsync(player.Id, items);
        }

        public async Task<DbQueryResult<InventoryItemModel>> RemoveItemAsync(long playerId, byte slotIndex) {
            var searchResult = await _inventoryRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType == DbResultType.Error || searchResult.ReturnValue == null) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Inventory not found.");
            }

            var inventory = searchResult.ReturnValue;
            if (slotIndex >= inventory.Items.Count) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Invalid slot index.");
            }

            var item = inventory.Items[slotIndex];
            inventory.Items.RemoveAt(slotIndex);
            var saveResult = await _inventoryRepository.SaveChangesAsync();
            if (!saveResult) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Failed to save inventory.");
            }

            return new DbQueryResult<InventoryItemModel>(DbResultType.Success, "Item removed successfully.", item);
        }

        public async Task<DbQueryResult<InventoryItemModel>> RemoveItemAsync(PlayerEntity player, byte slotIndex) {
            return await RemoveItemAsync(player.Id, slotIndex);
        }

        public async Task<DbQueryResult<InventoryItemModel>> ChangeItemSlotAsync(long playerId, byte oldSlotIndex, byte newSlotIndex) {
            var searchResult = await _inventoryRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType == DbResultType.Error || searchResult.ReturnValue == null) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Inventory not found.");
            }

            var inventory = searchResult.ReturnValue;
            if (oldSlotIndex >= inventory.Items.Count || newSlotIndex >= inventory.Items.Count) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Invalid slot index.");
            }

            var item = inventory.Items[oldSlotIndex];
            inventory.Items.RemoveAt(oldSlotIndex);
            inventory.Items.Insert(newSlotIndex, item);
            var saveResult = await _inventoryRepository.SaveChangesAsync();
            if (!saveResult) {
                return new DbQueryResult<InventoryItemModel>(DbResultType.Error, "Failed to save inventory.");
            }

            return new DbQueryResult<InventoryItemModel>(DbResultType.Success, "Item slot changed successfully.", item);
        }

        public async Task<DbQueryResult<InventoryItemModel>> ChangeItemSlotAsync(PlayerEntity player, byte oldSlotIndex, byte newSlotIndex) {
            return await ChangeItemSlotAsync(player.Id, oldSlotIndex, newSlotIndex);
        }

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

        public async Task<DbQueryResult<List<InventoryItemModel>>> ClearInventoryAsync(PlayerEntity player) {
            return await ClearInventoryAsync(player.Id);
        }
    }
}