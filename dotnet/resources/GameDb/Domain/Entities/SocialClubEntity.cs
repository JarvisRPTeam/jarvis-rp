using System.Collections.Generic;

namespace GameDb.Domain.Entities {
    public class SocialClubEntity {
        public long Id { get; set; } 
        public string Name { get; set; }
        public virtual ICollection<PlayerEntity> Players { get; set; }
        public virtual ICollection<InfrastructureBuildingEntity> InfrastructureBuildings { get; set; }
    }
}