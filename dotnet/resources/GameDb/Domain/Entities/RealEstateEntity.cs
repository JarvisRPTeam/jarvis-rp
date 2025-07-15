using System.Collections.Generic;
using GameDb.Domain.Models;

namespace GameDb.Domain.Entities
{
    public class RealEstateEntity
    {
        public long Id { get; set; }
        public RealEstateType Type { get; set; } 
        public long? OwnerId { get; set; } // Foreign key to Player
        public long AddressId { get; set; } // Foreign key to Address
        public byte MaxResidentCount { get; set; }
        public PositionModel SpawnPoint { get; set; } // JSONB

        // Navigation properties
        public virtual PlayerEntity Owner { get; set; } 
        public virtual AddressEntity Address { get; set; }
        public virtual ICollection<ResidenceEntity> Residences { get; set; }
    }

    public enum RealEstateType
    {
        House,
        Apartment,
    }
}