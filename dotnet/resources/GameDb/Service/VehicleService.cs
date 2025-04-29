using GameDb.Repository;
using GameDb.Domain;
using System;
using System.Threading.Tasks;

namespace GameDb.Service {
    public class VehicleService {
        private readonly IGameDbRepository<VehicleEntity> _vehicleRepository;
        private readonly GameDbContext _context;
        
        public VehicleService(IGameDbRepository<VehicleEntity> vehicleRepository, GameDbContext context) {
            _vehicleRepository = vehicleRepository;
            _context = context;
        }

        public async Task<DbQueryResult<VehicleEntity>> CreateVehicleAsync(VehicleCreateModel vehicleModel, ulong? playerId = null) {
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

        private string GenerateNumberPlate(ulong id) {
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