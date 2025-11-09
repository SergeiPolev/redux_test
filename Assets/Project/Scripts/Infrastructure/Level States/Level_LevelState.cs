using StateMachine;
using Services;

namespace Infrastructure
{
    public class Level_LevelState : IState
    {
        private IStateChanger _stateChanger;
        private WindowService _windowService;

        public Level_LevelState(IStateChanger stateChanger, IGameStateChanger gameStateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _windowService = services.Single<WindowService>();
        }
        public void Enter()
        {
            _windowService.Open(WindowId.HUD);
            //_windowService.Open(WindowId.Wallet);
        }

        public void Exit()
        {
            
        }
    }
}