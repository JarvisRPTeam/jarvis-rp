#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GameDb.Repository {
    public interface IInventoryRepository: IGameDbRepository<InventoryEntity> {
    }

    public class InventoryRepository : GameDbRepository<InventoryEntity>, IInventoryRepository {
        public InventoryRepository(GameDbContext context) : base(context) {
        }
    }
}
