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
    }

    public enum PunishmentType
    {
        Ban,
        Mute,
        Warning
    }
}