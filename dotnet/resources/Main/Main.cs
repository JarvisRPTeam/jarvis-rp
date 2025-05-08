using System;
using GTANetworkAPI;
using GameDb.Service;
using GameDb.Repository;
using Microsoft.Extensions.Configuration;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;

namespace Main
{
    public class Main: Script
    {
        private readonly GameDbContext _context;
        private readonly IGameDbRepository<PlayerEntity> _playerRepository;
        private readonly IPlayerService _playerService;
        private readonly IGameDbRepository<VehicleEntity> _vehicleRepository;
        private readonly IVehicleService _vehicleService;
        private readonly IGameDbRepository<SocialClubEntity> _socialClubRepository;

        public Main()
        {
            _context = new GameDbContext();

            _socialClubRepository = new GameDbRepository<SocialClubEntity>(_context);

            _playerRepository = new GameDbRepository<PlayerEntity>(_context);
            _playerService = new PlayerService(_playerRepository, _socialClubRepository);

            _vehicleRepository = new GameDbRepository<VehicleEntity>(_context);
            _vehicleService = new VehicleService(_vehicleRepository, _context);
            NAPI.Util.ConsoleOutput("Main Started with GameDbService.");
        }
    }
}
