using StateMachine;
using Services;

namespace Infrastructure
{
    internal class Game_LoadLevelState : IState
    {
        private IGameStateChanger _stateChanger;
        private GameWalletService _wallet;

        public Game_LoadLevelState(IGameStateChanger stateChanger, AllServices services) 
        {
            _stateChanger = stateChanger;
            _wallet = services.Single<GameWalletService>();
        }

        public void Enter()
        {
            _wallet.LoadLevel();
            _stateChanger.Enter<Game_LevelState>();
        }

        public void Exit()
        {

        }

    }
}