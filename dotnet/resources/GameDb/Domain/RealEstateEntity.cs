namespace GameDb.Domain
{
    public class RealEstateEntity
    {
        public ulong Id { get; set; }
        public RealEstateType Type { get; set; } 
        public ulong? OwnerId { get; set; } // Foreign key to Player
        public ulong AddressId { get; set; } // Foreign key to Address
        public uint PermittedVehicleCount { get; set; } // Number of vehicles allowed on the property
    }

    public enum RealEstateType
    {
        House,
        Apartment,
        Garage,
    }
}