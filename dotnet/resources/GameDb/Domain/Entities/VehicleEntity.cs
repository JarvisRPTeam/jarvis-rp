using GameDb.Domain.Models;

namespace GameDb.Domain.Entities
{
    public class VehicleEntity {
        public long Id { get; set; }
        public string Model { get; set; }
        public string NumberPlate { get; set; }
        public long? OwnerId { get; set; } // Foreign key to Player
        public float? CurrentFuel { get; set; } // If null, infinite fuel
        public float TankCapacity { get; set; } 
        public float FuelConsumption { get; set; } // l/km
        public float Mileage { get; set; } 
        public PositionModel Position { get; set; } // JSONB
        public VehicleColorModel Color { get; set; } // JSONB

        // Navigation properties
        public virtual PlayerEntity Owner { get; set; }
    }
}