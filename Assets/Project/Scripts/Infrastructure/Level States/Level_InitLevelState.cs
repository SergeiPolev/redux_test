using StateMachine;
using Services;

namespace Infrastructure
{
    public class Level_InitLevelState : IState
    {
        private IStateChanger _stateChanger;
        private readonly IGameStateChanger _gameStateChanger;
        private WindowService _windowService;
        private LevelProgressService _levelProgress;
        private InputService _inputService;
        private GameWalletService _wallet;

        public Level_InitLevelState(IStateChanger stateChanger, IGameStateChanger gameStateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            this._gameStateChanger = gameStateChanger;
            _windowService = services.Single<WindowService>();
            _levelProgress = services.Single<LevelProgressService>();
            _inputService = services.Single<InputService>();
            _wallet = services.Single<GameWalletService>();
        }
        public void Enter()
        {
            _levelProgress.LevelStarted();
            _windowService.Open(WindowId.LoadingScreen);

            _stateChanger.Enter<Level_StartLevelState>();
        }
        
        public void Exit()
        {
            _windowService.Close(WindowId.LoadingScreen);
        }
    }
}