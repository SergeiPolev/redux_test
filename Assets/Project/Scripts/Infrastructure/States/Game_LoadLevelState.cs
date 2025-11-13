using Infrastructure.Services;
using Infrastructure.StateMachine;

namespace Infrastructure.States
{
    internal class Game_LoadLevelState : IState
    {
        private readonly IGameStateChanger _stateChanger;

        public Game_LoadLevelState(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
        }

        public void Enter()
        {
            _stateChanger.Enter<Game_LevelState>();
        }

        public void Exit()
        {
        }
    }
}