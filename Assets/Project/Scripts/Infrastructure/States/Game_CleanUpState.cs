using StateMachine;
using Services;

namespace Infrastructure
{
    internal class Game_CleanUpState :  IState
    {
        private IGameStateChanger _stateChanger;
        private WindowService _windowService;
        private GameWalletService _wallet;

        public Game_CleanUpState(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _windowService = services.Single<WindowService>();
            _wallet = services.Single<GameWalletService>();
        }

        public void Enter()
        {
            _wallet.Cleanup();
            SetState();
        }
        
        
        public void Exit()
        {

        }

        private void SetState()
        {
            _stateChanger.Enter<Game_HubState>();
        }
    }
}