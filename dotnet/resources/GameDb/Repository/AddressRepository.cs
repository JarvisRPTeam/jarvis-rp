using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace GameDb.Repository {
    public interface IAddressRepository: IGameDbRepository<AddressEntity> {
        Task<DbQueryResult<IEnumerable<AddressEntity>>> GetByAddressNameAsync(string addressName);
    }

    public class AddressRepository: GameDbRepository<AddressEntity>, IAddressRepository {
        public AddressRepository(GameDbContext context) : base(context) {
        }

        public async Task<DbQueryResult<IEnumerable<AddressEntity>>> GetByAddressNameAsync(string addressName) {
            try {
                List<AddressEntity> addresses = await _dbSet
                    .Where(a => a.AddressName == addressName)
                    .ToListAsync();
                if (addresses.Count == 0) {
                    return new DbQueryResult<IEnumerable<AddressEntity>>(DbResultType.Warning, "No addresses found.");
                }
                return new DbQueryResult<IEnumerable<AddressEntity>>(DbResultType.Success, "Addresses found successfully.", addresses);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<AddressEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }
    }
}