using Services;

namespace Infrastructure
{
    public class ResultService : IService
    {
        private IStateChanger _gameStateChanger;
        private LevelStateMachine _levelStateMachine;
        private LevelProgressService _levelProgressService;
        private WindowService _windowService;
        
        public void Initialize(IStateChanger gameStateChanger)
        {
            _gameStateChanger = gameStateChanger;
            _levelProgressService = AllServices.Container.Single<LevelProgressService>();
        }

        public void SetLevelStateChanger(LevelStateMachine levelStateMachine)
        {
            _levelStateMachine = levelStateMachine;
        }
        
        public void Win()
        {
            _gameStateChanger.Enter<Game_ResultState, LevelResult>(LevelResult.WIN);
        }

        public void Lose()
        {
            _gameStateChanger.Enter<Game_ResultState, LevelResult>(LevelResult.LOSE);
        }
    }
}