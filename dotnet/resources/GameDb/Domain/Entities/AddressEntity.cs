using System.Collections.Generic;

namespace GameDb.Domain.Entities
{
    public class AddressEntity {
        public long Id { get; set; }
        public string AddressName { get; set; } 
        public virtual ICollection<RealEstateEntity> RealEstates { get; set; }
        public virtual InfrastructureBuildingEntity InfrastructureBuilding { get; set; }
    }
}