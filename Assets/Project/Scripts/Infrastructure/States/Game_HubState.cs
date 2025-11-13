using Infrastructure.Services;
using Infrastructure.Services.Core;
using Infrastructure.StateMachine;
using UI;
using UI.FadeWindow;
using UI.HubWindow;

namespace Infrastructure.States
{
    internal class Game_HubState : IState
    {
        private readonly FadeWindowModel _fadeWindowModel;
        private readonly IGameStateChanger _stateChanger;
        private readonly WindowService _windowService;
        private TransitionFadeWindow _uiFade;

        private HubWindow _uiHub;

        public Game_HubState(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _windowService = services.Single<WindowService>();
            _fadeWindowModel = new FadeWindowModel(0f, 0.5f, 0.1f, 0.5f, SetState);
        }

        public void Enter()
        {
            if (!_windowService.Open(WindowId.HUB, out _uiHub))
            {
                _uiHub = _windowService.GetOrCreateWindow<HubWindow>(WindowId.HUB);
            }

            _uiHub.OnGameStartEvent += Window_OnGameStartEvent;
        }


        public void Exit()
        {
            _uiHub.OnGameStartEvent -= Window_OnGameStartEvent;
            _windowService.Close(WindowId.HUB);
        }

        private void Window_OnGameStartEvent()
        {
            _windowService.Open(WindowId.TransitionFade, out _uiFade);
            _uiFade.SetModel(_fadeWindowModel);
        }


        private void SetState()
        {
            _stateChanger.Enter<Game_LoadLevelState>();
        }
    }
}