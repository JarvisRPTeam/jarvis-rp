namespace GameDb.Domain.Entities {
    public class ResidenceEntity {
        public ulong PlayerId { get; set; } // Foreign key to Player
        public ulong RealEstateId { get; set; } // Foreign key to RealEstate

        // Navigation properties
        public virtual PlayerEntity Player { get; set; }
        public virtual RealEstateEntity RealEstate { get; set; } 
    }
}