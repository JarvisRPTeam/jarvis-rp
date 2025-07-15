using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace GameDb.Repository
{
    public interface IPropertyRepository<TEntity> : IGameDbRepository<TEntity>
        where TEntity : class
    {
        Task<DbQueryResult<IEnumerable<TEntity>>> GetByOwnerIdAsync(long ownerId);
        Task<DbQueryResult<TEntity>> GetByAddressIdAsync(long addressId);
    }

    public class RealEstateRepository : GameDbRepository<RealEstateEntity>, IPropertyRepository<RealEstateEntity>
    {
        public RealEstateRepository(GameDbContext context) : base(context)
        {
        }

        public async Task<DbQueryResult<IEnumerable<RealEstateEntity>>> GetByOwnerIdAsync(long ownerId)
        {
            try
            {
                var realEstates = await _dbSet
                    .Where(r => r.OwnerId == ownerId)
                    .ToListAsync();
                if (realEstates.Count == 0)
                {
                    return new DbQueryResult<IEnumerable<RealEstateEntity>>(DbResultType.Warning, "No real estates found.");
                }
                return new DbQueryResult<IEnumerable<RealEstateEntity>>(DbResultType.Success, "Real estates found successfully.", realEstates);
            }
            catch (Exception ex)
            {
                return new DbQueryResult<IEnumerable<RealEstateEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<RealEstateEntity>> GetByAddressIdAsync(long addressId)
        {
            try
            {
                var realEstate = await _dbSet
                    .FirstOrDefaultAsync(r => r.AddressId == addressId);
                if (realEstate == null)
                {
                    return new DbQueryResult<RealEstateEntity>(DbResultType.Warning, "Real estate not found.");
                }
                return new DbQueryResult<RealEstateEntity>(DbResultType.Success, "Real estate found successfully.", realEstate);
            }
            catch (Exception ex)
            {
                return new DbQueryResult<RealEstateEntity>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }
    }
    
    public class GarageRepository : GameDbRepository<GarageEntity>, IPropertyRepository<GarageEntity>
    {
        public GarageRepository(GameDbContext context) : base(context)
        {
        }

        public async Task<DbQueryResult<IEnumerable<GarageEntity>>> GetByOwnerIdAsync(long ownerId)
        {
            try
            {
                var garages = await _dbSet
                    .Where(g => g.OwnerId == ownerId)
                    .ToListAsync();
                if (garages.Count == 0)
                {
                    return new DbQueryResult<IEnumerable<GarageEntity>>(DbResultType.Warning, "No garages found.");
                }
                return new DbQueryResult<IEnumerable<GarageEntity>>(DbResultType.Success, "Garages found successfully.", garages);
            }
            catch (Exception ex)
            {
                return new DbQueryResult<IEnumerable<GarageEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<GarageEntity>> GetByAddressIdAsync(long addressId)
        {
            try
            {
                var garage = await _dbSet
                    .FirstOrDefaultAsync(g => g.AddressId == addressId);
                if (garage == null)
                {
                    return new DbQueryResult<GarageEntity>(DbResultType.Warning, "Garage not found.");
                }
                return new DbQueryResult<GarageEntity>(DbResultType.Success, "Garage found successfully.", garage);
            }
            catch (Exception ex)
            {
                return new DbQueryResult<GarageEntity>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }
    }
}