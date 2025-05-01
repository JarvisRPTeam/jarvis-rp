namespace GameDb.Domain.Entities
{
    public class VehicleEntity {
        public ulong Id { get; set; }
        public string Model { get; set; }
        public string NumberPlate { get; set; }
        public ulong? OwnerId { get; set; } // Foreign key to Player

        // Navigation properties
        public virtual PlayerEntity Owner { get; set; }
    }
}