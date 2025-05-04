namespace GameDb.Domain.Entities {
    public class InfrastructureBuildingEntity {
        public ulong Id { get; set; } 
        public string Name { get; set; }
        public ulong SocialClubId { get; set; }
        public ulong AddressId { get; set; }

        // Navigation properties
        public virtual SocialClubEntity SocialClub { get; set; } 
        public virtual AddressEntity Address { get; set; }
    }
}