#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GameDb.Repository {
    public interface IGameDbRepository<TEntity> where TEntity : class {
        Task<DbQueryResult<TEntity>> GetByIdAsync(ulong id);
        Task<DbQueryResult<IEnumerable<TEntity>>> GetAllAsync();
        Task<DbQueryResult<TEntity>> AddAsync(TEntity entity);
        Task<DbQueryResult<TEntity>> DeleteByIdAsync(ulong id);
        Task<bool> SaveChangesAsync();
    }

    public class GameDbRepository<TEntity> : IGameDbRepository<TEntity> where TEntity : class {
        private readonly GameDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GameDbRepository(GameDbContext context) {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<DbQueryResult<TEntity>> GetByIdAsync(ulong id) {
            try {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null) {
                    return new DbQueryResult<TEntity>(DbResultType.Error, "Entity not found.");
                }
                return new DbQueryResult<TEntity>(DbResultType.Success, "Entity found.", entity);
            } catch (Exception ex) {
                return new DbQueryResult<TEntity>(DbResultType.Error, $"Error retrieving entity: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<IEnumerable<TEntity>>> GetAllAsync() {
            try {
                var entities = await _dbSet.ToListAsync();
                return new DbQueryResult<IEnumerable<TEntity>>(DbResultType.Success, "Entities retrieved successfully.", entities);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<TEntity>>(DbResultType.Error, $"Error retrieving entities: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<TEntity>> AddAsync(TEntity entity) {
            try {
                await _dbSet.AddAsync(entity);
                return new DbQueryResult<TEntity>(DbResultType.Success, "Entity added successfully.");
            } catch (Exception ex) {
                return new DbQueryResult<TEntity>(DbResultType.Error, $"Error adding entity: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<TEntity>> DeleteByIdAsync(ulong id) {
            var searchResult = await GetByIdAsync(id);
            if (searchResult.ReturnValue == null) {
                return new DbQueryResult<TEntity>(DbResultType.Error, searchResult.Message);
            }
            if (searchResult.ResultType == DbResultType.Error) {
                return searchResult;
            }

            var entity = searchResult.ReturnValue;
            try {
                _dbSet.Remove(entity);
                return new DbQueryResult<TEntity>(DbResultType.Success, "Entity deleted successfully.");
            } catch (Exception ex) {
                return new DbQueryResult<TEntity>(DbResultType.Error, $"Error deleting entity: {ex.Message}");
            }
        }

        public async Task<bool> SaveChangesAsync() {
            try {
                return await _context.SaveChangesAsync() > 0;
            } catch (DbUpdateException ex) {
                Console.WriteLine($"Error saving changes: {ex.Message}");
                return false;
            }
        }
    }
}