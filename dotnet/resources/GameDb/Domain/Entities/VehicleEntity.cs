namespace GameDb.Domain.Entities
{
    public class VehicleEntity {
        public long Id { get; set; }
        public string Model { get; set; }
        public string NumberPlate { get; set; }
        public long? OwnerId { get; set; } // Foreign key to Player

        // Navigation properties
        public virtual PlayerEntity Owner { get; set; }
    }
}