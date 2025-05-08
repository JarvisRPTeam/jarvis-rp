namespace GameDb.Domain.Entities
{
    public class PlayerEntity
    {
        public long Id { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public long Cash { get; set; }
        public byte HP { get; set; }
        public byte Hunger { get; set; }
        public byte Thirst { get; set; }
        public byte Stamina { get; set; }
        public long? SocialClubId { get; set; } // Foreign key to SocialClub
        public float PositionX { get; set; }
        public float PositionY { get; set; } 
        public float PositionZ { get; set; } 

        // Navigation properties
        public virtual SocialClubEntity SocialClub { get; set; }
    }
}