using GameDb.Repository;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameDb.Service {
    public interface IVehicleService
    {
        Task<VehicleEntity> CreateVehicleAsync(VehicleCreateModel vehicleModel, long? playerId = null);
        Task<bool> RemoveVehicleAsync(VehicleEntity vehicleEntity);
        Task<bool> AssignOwnerAsync(VehicleEntity vehicleEntity, long playerId);
        Task<bool> RemoveOwnerAsync(VehicleEntity vehicleId);
        Task<VehicleEntity> GetVehicleByIdAsync(long vehicleId);
        Task<IEnumerable<VehicleEntity>> GetVehiclesByOwnerIdAsync(long ownerId);
        Task<IEnumerable<VehicleEntity>> GetVehiclesByModelAsync(string model);
        Task<IEnumerable<VehicleEntity>> GetVehiclesByNumberPlateAsync(string numberPlate);
    }
    
    public class VehicleService: IVehicleService {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly GameDbContext _context;
        
        public VehicleService(IVehicleRepository vehicleRepository, GameDbContext context) {
            _vehicleRepository = vehicleRepository;
            _context = context;
        }

        public async Task<VehicleEntity> CreateVehicleAsync(VehicleCreateModel vehicleModel, long? playerId = null) {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try {
                var vehicleEntity = new VehicleEntity {
                    Model = vehicleModel.Model,
                    NumberPlate = vehicleModel.NumberPlate,
                    OwnerId = playerId
                };

                var addResult = await _vehicleRepository.AddAsync(vehicleEntity);
                if (addResult.ResultType != DbResultType.Success) {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error adding vehicle: {addResult.Message}");
                    return null;
                }

                // Save to get the ID assigned
                var saved = await _vehicleRepository.SaveChangesAsync();
                if (!saved) {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Failed to save vehicle to database.");
                    return null;
                }

                // Generate plate based on ID if not provided
                if (vehicleEntity.NumberPlate == null) {
                    vehicleEntity.NumberPlate = GenerateNumberPlate(vehicleEntity.Id);
                }
                saved = await _vehicleRepository.SaveChangesAsync();
                if (!saved) {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Failed to save vehicle with generated number plate.");
                    return null;
                }

                await transaction.CommitAsync();
                return vehicleEntity;
            }
            catch (Exception ex) {
                await transaction.RollbackAsync();
                Console.WriteLine($"Exception while creating vehicle: {ex.Message}");
                return null;
            }
        }
        
        public async Task<bool> RemoveVehicleAsync(VehicleEntity vehicleEntity) {
            try {
                var deleteResult = await _vehicleRepository.DeleteByIdAsync(vehicleEntity.Id);
                if (deleteResult.ResultType != DbResultType.Success) {
                    Console.WriteLine($"Error removing vehicle: {deleteResult.Message}");
                    return false;
                }

                var saved = await _vehicleRepository.SaveChangesAsync();
                if (!saved) {
                    Console.WriteLine("Failed to save changes after deleting vehicle.");
                    return false;
                }
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine($"Exception while removing vehicle: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AssignOwnerAsync(VehicleEntity vehicleEntity, long playerId)
        {
            vehicleEntity.OwnerId = playerId;
            var updateResult = await _vehicleRepository.SaveChangesAsync();
            if (!updateResult)
            {
                Console.WriteLine("Failed to assign owner to vehicle.");
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveOwnerAsync(VehicleEntity vehicleEntity)
        {
            vehicleEntity.OwnerId = null;
            var updateResult = await _vehicleRepository.SaveChangesAsync();
            if (!updateResult)
            {
                Console.WriteLine("Failed to remove owner from vehicle.");
                return false;
            }
            return true;
        }

        public async Task<VehicleEntity> GetVehicleByIdAsync(long vehicleId) {
            var result = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (result.ResultType == DbResultType.Error || result.ReturnValue == null) {
                Console.WriteLine("Vehicle not found.");
                return null;
            }
            if (result.ResultType == DbResultType.Warning) {
                Console.WriteLine(result.Message);
            }
            return result.ReturnValue;
        }

        public async Task<IEnumerable<VehicleEntity>> GetVehiclesByOwnerIdAsync(long ownerId) {
            var result = await _vehicleRepository.GetByOwnerIdAsync(ownerId);
            if (result.ResultType == DbResultType.Error) {
                Console.WriteLine($"Error retrieving vehicles by player: {result.Message}");
                return null;
            }
            if (result.ResultType == DbResultType.Warning) {
                Console.WriteLine(result.Message);
            }
            return result.ReturnValue;
        }

        public async Task<IEnumerable<VehicleEntity>> GetVehiclesByModelAsync(string model) {
            var result = await _vehicleRepository.GetByModelAsync(model);
            if (result.ResultType == DbResultType.Error) {
                Console.WriteLine($"Error retrieving vehicles by model: {result.Message}");
                return null;
            }
            if (result.ResultType == DbResultType.Warning) {
                Console.WriteLine(result.Message);
            }
            return result.ReturnValue;
        }

        public async Task<IEnumerable<VehicleEntity>> GetVehiclesByNumberPlateAsync(string numberPlate) {
            var result = await _vehicleRepository.GetByNumberPlateAsync(numberPlate);
            if (result.ResultType == DbResultType.Error) {
                Console.WriteLine($"Error retrieving vehicles by number plate: {result.Message}");
                return null;
            }
            if (result.ResultType == DbResultType.Warning) {
                Console.WriteLine(result.Message);
            }
            return result.ReturnValue;
        }

        private string GenerateNumberPlate(long id)
        {
            if (id > 6759326)
            {
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