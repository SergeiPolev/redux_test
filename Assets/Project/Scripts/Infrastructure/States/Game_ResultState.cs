using StateMachine;
using Services;

public enum LevelResult
{
    WIN,
    LOSE,
    LEAVE
}

namespace Infrastructure
{
    internal class Game_ResultState : ITick, IPayloadedState<LevelResult>
    {
        private WindowService _windowService;
        private IGameStateChanger _stateChanger;
        protected string _result;

        public Game_ResultState(IGameStateChanger stateChanger, AllServices services) 
        {
            _stateChanger = stateChanger;
            _windowService = services.Single<WindowService>();
        }

        public void Tick()
        {
            
        }

        public void Enter(LevelResult result)
        {
            if (result == LevelResult.WIN)
            {
                AllServices.Container.Single<LevelProgressService>().LevelComplete();
                _windowService.GetOrCreateWindow<UIWin>(WindowId.Win).SetOnHubEvent(OnHub);
                _windowService.Open(WindowId.Win);
            }
            else
            {
                _windowService.Open(WindowId.Lose);
                _windowService.GetOrCreateWindow<UILose>(WindowId.Lose).SetOnHubEvent(OnHub);
            }
        }

        private void OnHub()
        {
            _stateChanger.Enter<Game_CleanUpState>();
        }

        public void Exit()
        {

        }
    }
}