using Infrastructure.Services;
using Infrastructure.Services.Core;
using Infrastructure.StateMachine;

namespace Infrastructure.States
{
    internal class Game_CleanUpState : IState
    {
        private readonly IGameStateChanger _stateChanger;
        private WindowService _windowService;

        public Game_CleanUpState(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _windowService = services.Single<WindowService>();
        }

        public void Enter()
        {
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