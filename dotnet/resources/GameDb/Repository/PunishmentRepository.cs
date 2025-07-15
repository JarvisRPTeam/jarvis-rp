using GameDb.Domain.Entities;

namespace GameDb.Repository
{
    public interface IPunishmentRepository : IGameDbRepository<PunishmentEntity>
    {

    }

    public class PunishmentRepository : GameDbRepository<PunishmentEntity>, IPunishmentRepository
    {
        public PunishmentRepository(GameDbContext context) : base(context)
        {
        }

    }
}
    