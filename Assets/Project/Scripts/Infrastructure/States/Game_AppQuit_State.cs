using Infrastructure.Services;
using Infrastructure.StateMachine;

namespace Infrastructure.States
{
    public class Game_AppQuit_State : IState
    {
        private readonly AllServices _services;
        private IGameStateChanger _stateChanger;

        public Game_AppQuit_State(IGameStateChanger stateChanger, AllServices services)
        {
            _services = services;
            _stateChanger = stateChanger;
        }

        public void Enter()
        {
            foreach (var disposable in _services.Disposables)
            {
                disposable.Dispose();
            }
        }

        public void Exit()
        {
        }
    }
}