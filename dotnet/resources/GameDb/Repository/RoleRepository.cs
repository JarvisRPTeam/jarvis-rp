#nullable enable
using System;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameDb.Repository
{
    public interface IRoleRepository : IGameDbRepository<RoleEntity>
    {
        Task<DbQueryResult<RoleEntity>> GetByNameAsync(string name);
    }

    public class RoleRepository : GameDbRepository<RoleEntity>, IRoleRepository
    {
        public RoleRepository(GameDbContext context) : base(context)
        {
        }

        public async Task<DbQueryResult<RoleEntity>> GetByNameAsync(string name)
        {
            try
            {
                RoleEntity? role = await _dbSet
                    .FirstOrDefaultAsync(r => r.Name == name);
                if (role == null)
                {
                    return new DbQueryResult<RoleEntity>(DbResultType.Warning, "Role not found.");
                }
                return new DbQueryResult<RoleEntity>(DbResultType.Success, "Role found successfully.", role);
            }
            catch (Exception ex)
            {
                return new DbQueryResult<RoleEntity>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }
    }
}