using System;
using GameDb.Domain.Entities;

namespace GameDb.Domain.Models
{
    public class PunishmentCreateModel
    {
        public long PlayerId { get; set; }
        public string Reason { get; set; }
        public TimeSpan Duration { get; set; }
        public long AdminId { get; set; }
        public PunishmentType Type { get; set; }
    }
}