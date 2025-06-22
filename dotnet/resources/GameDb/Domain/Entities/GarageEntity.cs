using GameDb.Domain.Models;

namespace GameDb.Domain.Entities
{
    public class GarageEntity
    {
        public long Id { get; set; }
        public long? OwnerId { get; set; } // Foreign key to Player
        public long AddressId { get; set; } // Foreign key to Address
        public byte MaxVehicleCount { get; set; }
        public PositionModel[] VehicleSpawnPoints { get; set; } // JSONB [[x, y, z, heading], ...]

        // Navigation properties
        public virtual PlayerEntity Owner { get; set; }
        public virtual AddressEntity Address { get; set; }
    }
}