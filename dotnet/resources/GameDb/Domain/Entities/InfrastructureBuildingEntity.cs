namespace GameDb.Domain.Entities {
    public class InfrastructureBuildingEntity {
        public long Id { get; set; } 
        public string Name { get; set; }
        public long SocialClubId { get; set; }
        public long AddressId { get; set; }

        // Navigation properties
        public virtual SocialClubEntity SocialClub { get; set; } 
        public virtual AddressEntity Address { get; set; }
    }
}