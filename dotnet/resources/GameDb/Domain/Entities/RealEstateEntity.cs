namespace GameDb.Domain.Entities
{
    public class RealEstateEntity
    {
        public long Id { get; set; }
        public RealEstateType Type { get; set; } 
        public long? OwnerId { get; set; } // Foreign key to Player
        public long AddressId { get; set; } // Foreign key to Address
        public byte MaxVehicleCount { get; set; } 
        public byte MaxResidentCount { get; set; }

        // Navigation properties
        public virtual PlayerEntity Owner { get; set; } 
        public virtual AddressEntity Address { get; set; }
    }

    public enum RealEstateType
    {
        House,
        Apartment,
        Garage,
    }
}