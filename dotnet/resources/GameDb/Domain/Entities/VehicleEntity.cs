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
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float Heading { get; set; } 

        // Navigation properties
        public virtual PlayerEntity Owner { get; set; }
    }
}