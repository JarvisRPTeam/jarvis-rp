using System.Collections.Generic;

namespace GameDb.Domain.Entities
{
    public class RoleEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Permissions { get; set; } // JSONB
    }
} 
