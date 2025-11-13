using Infrastructure.Services;
using Infrastructure.Services.Core;
using Infrastructure.Services.Progress;
using Infrastructure.StateMachine;
using UI;
using UI.Results;

namespace Infrastructure.States
{
    public enum LevelResult
    {
        WIN,
        LOSE,
        LEAVE,
    }

    internal class Game_ResultState : ITick, IPayloadedState<LevelResult>
    {
        private readonly IGameStateChanger _stateChanger;
        private readonly WindowService _windowService;
        protected string _result;

        public Game_ResultState(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _windowService = services.Single<WindowService>();
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

        public void Exit()
        {
        }

        public void Tick()
        {
        }

        private void OnHub()
        {
            _stateChanger.Enter<Game_CleanUpState>();
        }
    }
}