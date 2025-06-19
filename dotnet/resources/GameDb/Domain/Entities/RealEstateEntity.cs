namespace GameDb.Domain.Entities
{
    public class RealEstateEntity
    {
        public long Id { get; set; }
        public RealEstateType Type { get; set; } 
        public long? OwnerId { get; set; } // Foreign key to Player
        public long AddressId { get; set; } // Foreign key to Address
        public byte MaxResidentCount { get; set; }
        public float SpawnPointX { get; set; }
        public float SpawnPointY { get; set; }
        public float SpawnPointZ { get; set; }
        public float SpawnPointHeading { get; set; }

        // Navigation properties
        public virtual PlayerEntity Owner { get; set; } 
        public virtual AddressEntity Address { get; set; }
    }

    public enum RealEstateType
    {
        House,
        Apartment,
    }
}