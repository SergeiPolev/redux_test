using StateMachine;
using Services;

namespace Infrastructure
{
    internal class Game_HubState : IState
    {
        private WindowService _windowService;
        private IGameStateChanger _stateChanger;
        private FadeWindowModel _fadeWindowModel;

        private HubWindow _uiHub;
        private TransitionFadeWindow _uiFade;

        public Game_HubState(IGameStateChanger stateChanger, AllServices services) 
        {
            _stateChanger = stateChanger;
            _windowService = services.Single<WindowService>();
            _fadeWindowModel = new FadeWindowModel(0f, 0.5f, 0.1f, 0.5f, SetState);
        }

        public void Enter()
        {
            if (_windowService.Open(WindowId.HUB, out _uiHub) == false)
                _uiHub = _windowService.GetOrCreateWindow<HubWindow>(WindowId.HUB);
            _uiHub.OnGameStartEvent += Window_OnGameStartEvent;

            _windowService.Open(WindowId.HUB_Navigation);
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



        public void Exit()
        {
            _uiHub.OnGameStartEvent -= Window_OnGameStartEvent;
            _windowService.Close(WindowId.HUB);
            _windowService.Close(WindowId.HUB_Navigation);
        }
    }
}

