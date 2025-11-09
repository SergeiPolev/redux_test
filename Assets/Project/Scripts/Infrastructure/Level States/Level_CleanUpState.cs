using StateMachine;
using Services;

namespace Infrastructure
{
    public class Level_CleanUpState : IState
    {
        private IStateChanger _stateChanger;
        private WindowService _windowService;

        public Level_CleanUpState(IStateChanger stateChanger, IGameStateChanger gameStateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _windowService = services.Single<WindowService>();
        }
        public void Enter()
        {
#if DEBUG
            _windowService.Close(WindowId.HUD_Debug);
#endif
            _windowService.Close(WindowId.HUD);
            _windowService.Close(WindowId.Wallet);
        }

        public void Exit()
        {
        }
    }
}