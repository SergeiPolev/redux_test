using Services;
using StateMachine;

namespace Infrastructure
{
    public class Game_AppQuit_State : IState
    {
        private IGameStateChanger _stateChanger;

        public Game_AppQuit_State(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
        }
        public void Enter()
        {
            
        }

        public void Exit()
        {
            
        }
    }
}