using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace GameDb.Repository {
    public interface IVehicleRepository: IGameDbRepository<VehicleEntity> {
        Task<DbQueryResult<IEnumerable<VehicleEntity>>> GetByOwnerIdAsync(long ownerId);
        Task<DbQueryResult<IEnumerable<VehicleEntity>>> GetByModelAsync(string model);
        Task<DbQueryResult<IEnumerable<VehicleEntity>>> GetByNumberPlateAsync(string numberPlate);
    }

    public class VehicleRepository: GameDbRepository<VehicleEntity>, IVehicleRepository {
        public VehicleRepository(GameDbContext context) : base(context) {
        }

        public async Task<DbQueryResult<IEnumerable<VehicleEntity>>> GetByOwnerIdAsync(long ownerId) {
            try {
                var vehicles = await _dbSet
                    .Where(v => v.OwnerId == ownerId)
                    .ToListAsync();
                if (vehicles.Count == 0) {
                    return new DbQueryResult<IEnumerable<VehicleEntity>>(DbResultType.Warning, "No vehicles found.");
                }
                return new DbQueryResult<IEnumerable<VehicleEntity>>(DbResultType.Success, "Vehicles found successfully.", vehicles);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<VehicleEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<IEnumerable<VehicleEntity>>> GetByModelAsync(string model) {
            try {
                var vehicles = await _dbSet
                    .Where(v => v.Model == model)
                    .ToListAsync();
                if (vehicles.Count == 0) {
                    return new DbQueryResult<IEnumerable<VehicleEntity>>(DbResultType.Warning, "No vehicles found.");
                }
                return new DbQueryResult<IEnumerable<VehicleEntity>>(DbResultType.Success, "Vehicles found successfully.", vehicles);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<VehicleEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<IEnumerable<VehicleEntity>>> GetByNumberPlateAsync(string numberPlate) {
            try {
                var vehicles = await _dbSet
                    .Where(v => v.NumberPlate == numberPlate)
                    .ToListAsync();
                if (vehicles.Count == 0) {
                    return new DbQueryResult<IEnumerable<VehicleEntity>>(DbResultType.Warning, "No vehicles found.");
                }
                return new DbQueryResult<IEnumerable<VehicleEntity>>(DbResultType.Success, "Vehicles found successfully.", vehicles);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<VehicleEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }
    }
}