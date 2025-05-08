using GameDb.Repository;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;
using System;
using System.Threading.Tasks;

namespace GameDb.Service {
    public interface IVehicleService {
        Task<DbQueryResult<VehicleEntity>> CreateVehicleAsync(VehicleCreateModel vehicleModel, long? playerId = null);
        Task<DbQueryResult<VehicleEntity>> CreateVehicleAsync(VehicleCreateModel vehicleModel, PlayerEntity player);
        Task<DbQueryResult<VehicleEntity>> AssignOwnerAsync(VehicleEntity vehicleEntity, long playerId);
        Task<DbQueryResult<VehicleEntity>> AssignOwnerAsync(long vehicleId, long playerId);
        Task<DbQueryResult<VehicleEntity>> RemoveOwnerAsync(long vehicleId);
    }
    
    public class VehicleService: IVehicleService {
        private readonly IGameDbRepository<VehicleEntity> _vehicleRepository;
        private readonly GameDbContext _context;
        
        public VehicleService(IGameDbRepository<VehicleEntity> vehicleRepository, GameDbContext context) {
            _vehicleRepository = vehicleRepository;
            _context = context;
        }

        public async Task<DbQueryResult<VehicleEntity>> CreateVehicleAsync(VehicleCreateModel vehicleModel, long? playerId = null) {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try {
                var vehicleEntity = new VehicleEntity {
                    // Id is not set, assuming auto-increment by DB
                    Model = vehicleModel.Model,
                    NumberPlate = vehicleModel.NumberPlate,
                    OwnerId = playerId
                };

                var addResult = await _vehicleRepository.AddAsync(vehicleEntity);
                if (addResult.ResultType != DbResultType.Success) {
                    await transaction.RollbackAsync();
                    return addResult;
                }

                // Save to get the ID assigned
                var saved = await _vehicleRepository.SaveChangesAsync();
                if (!saved) {
                    await transaction.RollbackAsync();
                    return new DbQueryResult<VehicleEntity>(DbResultType.Error, "Failed to save vehicle to database.");
                }

                // Generate plate based on ID if not provided
                if (vehicleEntity.NumberPlate == null) {
                    vehicleEntity.NumberPlate = GenerateNumberPlate(vehicleEntity.Id);

                    saved = await _vehicleRepository.SaveChangesAsync();
                    if (!saved) {
                        await transaction.RollbackAsync();
                        return new DbQueryResult<VehicleEntity>(DbResultType.Error, "Failed to save vehicle with generated number plate.");
                    }
                }

                await transaction.CommitAsync();
                return new DbQueryResult<VehicleEntity>(DbResultType.Success, "Vehicle created successfully.", vehicleEntity);
            }
            catch (Exception ex) {
                await transaction.RollbackAsync();
                return new DbQueryResult<VehicleEntity>(DbResultType.Error, $"Error creating vehicle: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<VehicleEntity>> CreateVehicleAsync(VehicleCreateModel vehicleModel, PlayerEntity player) {
            return await CreateVehicleAsync(vehicleModel, player.Id);
        }

        public async Task<DbQueryResult<VehicleEntity>> AssignOwnerAsync(VehicleEntity vehicleEntity, long playerId) {
            vehicleEntity.OwnerId = playerId;
            var updateResult = await _vehicleRepository.SaveChangesAsync();
            if (!updateResult) {
                return new DbQueryResult<VehicleEntity>(DbResultType.Error, "Failed to assign owner to vehicle.");
            }
            return new DbQueryResult<VehicleEntity>(DbResultType.Success, "Owner assigned successfully.", vehicleEntity);
        }

        public async Task<DbQueryResult<VehicleEntity>> AssignOwnerAsync(long vehicleId, long playerId) {
            var vehicleResult = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicleResult.ResultType != DbResultType.Success) {
                return vehicleResult;
            }

            var assignResult = await AssignOwnerAsync(vehicleResult.ReturnValue, playerId);
            return assignResult;
        }

        public async Task<DbQueryResult<VehicleEntity>> RemoveOwnerAsync(long vehicleId) {
            var vehicleResult = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicleResult.ResultType != DbResultType.Success) {
                return vehicleResult;
            }

            var vehicleEntity = vehicleResult.ReturnValue;
            vehicleEntity.OwnerId = null; // Remove owner

            var updateResult = await _vehicleRepository.SaveChangesAsync();
            if (!updateResult) {
                return new DbQueryResult<VehicleEntity>(DbResultType.Error, "Failed to remove owner from vehicle.");
            }

            return new DbQueryResult<VehicleEntity>(DbResultType.Success, "Owner removed successfully.", vehicleEntity);
        }

        private string GenerateNumberPlate(long id) {
            if (id > 6759326) {
                throw new ArgumentOutOfRangeException(nameof(id), "ID exceeds the maximum value for number plate generation.");
            }
            
            char firstLetter = (char)('A' + (id / (26 * 9999)));
            
            char secondLetter = (char)('A' + (id % (26 * 9999) / 9999));
            
            int number = (int)(id % 9999) + 1; // +1 to make it 1-9999 instead of 0-9998
            
            // Format as "XX-0000"
            return $"{firstLetter}{secondLetter}-{number:D4}";
        }
    }
}