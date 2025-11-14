using Infrastructure.Services;
using Infrastructure.Services.Core;
using Infrastructure.Services.Gameplay;
using Infrastructure.StateMachine;
using UI;

namespace Infrastructure
{
    public class Level_LevelState : IState, ITick, ILateTick
    {
        private readonly BoosterService _boosterService;
        private readonly HexPilesService _hexPilesService;
        private readonly InputService _inputService;
        private readonly MergeService _mergeService;
        private readonly WindowService _windowService;

        public Level_LevelState(AllServices services)
        {
            _windowService = services.Single<WindowService>();
            _inputService = services.Single<InputService>();
            _hexPilesService = services.Single<HexPilesService>();
            _mergeService = services.Single<MergeService>();
            _boosterService = services.Single<BoosterService>();
        }

        public void Enter()
        {
            _windowService.Open(WindowId.HUD);
            _hexPilesService.OnLevelEnter();
            _mergeService.OnLevelEnter();
        }

        public void Exit()
        {
            _boosterService.Dispose();
        }

        public void Tick()
        {
            _boosterService.Tick();
        }

        public void LateTick()
        {
            _inputService.LateTick();
        }
    }
}