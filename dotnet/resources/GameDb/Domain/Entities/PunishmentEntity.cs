using System;

namespace GameDb.Domain.Entities
{
    public class PunishmentEntity
    {
        public long Id { get; set; }
        public long PlayerId { get; set; } // Foreign key to Player
        public string Reason { get; set; }
        public PunishmentType Type { get; set; }
        public DateTime? Timeout { get; set; } // Nullable for permanent punishments, timestamp of punishment end
        public long AdminId { get; set; } // Foreign key to Admin who issued the punishment
        // Navigation properties
        public virtual PlayerEntity Player { get; set; }
        public virtual PlayerEntity Admin { get; set; }
    }

    public enum PunishmentType
    {
        Ban,
        Mute,
        Warning
    }
}