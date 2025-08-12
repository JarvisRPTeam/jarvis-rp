using System;
using GameDb.Domain.Entities;

namespace GameDb.Domain.Models
{
    public class PunishmentDisplayModel
    {
        public PunishmentType Type { get; set; }
        public string Reason { get; set; }
        public DateTime? Timeout { get; set; }
    }
}