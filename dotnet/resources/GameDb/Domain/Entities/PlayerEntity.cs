using System;
using System.Collections.Generic;
using GameDb.Domain.Models;

namespace GameDb.Domain.Entities
{
    public class PlayerEntity
    {
        public long Id { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public long? SocialClubId { get; set; } // Foreign key to SocialClub
        public long RoleId { get; set; } // Foreign key to Role
        public long? SpawnPlaceId { get; set; } // Foreign key to RealEstate
        public long Cash { get; set; }
        public long? BankBalance { get; set; }
        public long? BankCardNumber { get; set; }
        public long? BankCardPIN { get; set; }
        public long JarvisBalance { get; set; }
        public byte HP { get; set; }
        public byte Hunger { get; set; }
        public byte Thirst { get; set; }
        public byte Stamina { get; set; }
        public byte Breath { get; set; }
        public PositionModel Position { get; set; } // JSONB
        public byte Strength { get; set; }
        public byte Endurance { get; set; }
        public byte Stealth { get; set; }
        public byte DrivingSkill { get; set; }
        public byte ShootingSkill { get; set; }
        public byte FishingSkill { get; set; }
        public byte HuntingSkill { get; set; }
        public byte FlyingSkill { get; set; }
        public byte BreathHoldingSkill { get; set; }
        public TimeSpan PlayedToday { get; set; }
        public TimeSpan PlayedTotal { get; set; }

        // Navigation properties
        public virtual SocialClubEntity SocialClub { get; set; }
        public virtual RoleEntity Role { get; set; }
        public virtual RealEstateEntity SpawnPlace { get; set; }
        public virtual InventoryEntity Inventory { get; set; }
        public virtual ResidenceEntity Residence { get; set; }
        public virtual ICollection<VehicleEntity> Vehicles { get; set; }
        public virtual ICollection<RealEstateEntity> RealEstates { get; set; }
        public virtual ICollection<PunishmentEntity> Punishments { get; set; }
        public virtual ICollection<GarageEntity> Garages { get; set; }
    }
}