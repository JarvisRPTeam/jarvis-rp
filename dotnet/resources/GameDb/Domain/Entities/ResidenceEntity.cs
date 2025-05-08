namespace GameDb.Domain.Entities {
    public class ResidenceEntity {
        public long PlayerId { get; set; } // Foreign key to Player
        public long RealEstateId { get; set; } // Foreign key to RealEstate

        // Navigation properties
        public virtual PlayerEntity Player { get; set; }
        public virtual RealEstateEntity RealEstate { get; set; } 
    }
}