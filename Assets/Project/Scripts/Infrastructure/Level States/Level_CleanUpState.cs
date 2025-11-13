using Infrastructure.Services;
using Infrastructure.Services.Core;
using Infrastructure.Services.Gameplay;
using Infrastructure.StateMachine;
using UI;

namespace Infrastructure
{
    public class Level_CleanUpState : IState
    {
        private readonly IHexGridService _gridService;
        private readonly HexPilesService _pilesService;
        private readonly ResultService _resultService;
        private readonly WindowService _windowService;
        private IStateChanger _stateChanger;

        public Level_CleanUpState(IStateChanger stateChanger, IGameStateChanger gameStateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _windowService = services.Single<WindowService>();
            _gridService = services.Single<IHexGridService>();
            _resultService = services.Single<ResultService>();
            _pilesService = services.Single<HexPilesService>();
        }

        public void Enter()
        {
            _windowService.Close(WindowId.HUD);

            _resultService.OnLevelExit();
            _gridService.CleanUpGrid();
            _pilesService.ClearPiles();
        }

        public void Exit()
        {
        }
    }
}