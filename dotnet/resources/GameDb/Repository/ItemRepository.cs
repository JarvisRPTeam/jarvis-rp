using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace GameDb.Repository {
    public interface IItemRepository: IGameDbRepository<ItemEntity> {
        Task<DbQueryResult<IEnumerable<ItemEntity>>> GetByItemNameAsync(string itemName);
    }

    public class ItemRepository: GameDbRepository<ItemEntity>, IItemRepository {
        public ItemRepository(GameDbContext context) : base(context) {
        }

        public async Task<DbQueryResult<IEnumerable<ItemEntity>>> GetByItemNameAsync(string itemName) {
            try {
                List<ItemEntity> items = await _dbSet
                    .Where(i => i.Name == itemName)
                    .ToListAsync();
                if (items.Count == 0) {
                    return new DbQueryResult<IEnumerable<ItemEntity>>(DbResultType.Warning, "No items found.");
                }
                return new DbQueryResult<IEnumerable<ItemEntity>>(DbResultType.Success, "Items found successfully.", items);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<ItemEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }
    }
}