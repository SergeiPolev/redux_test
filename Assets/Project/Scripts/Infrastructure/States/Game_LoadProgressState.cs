using StateMachine;
using Services;
using Data;

namespace Infrastructure
{
    internal class Game_LoadProgressState : IState
    {
        private SaveLoadService _saveLoadService;
        private IGameStateChanger _stateChanger;
        private WindowService _windowService;
        //private TutorialService _tutorialService;
        private AllServices _services;

        public Game_LoadProgressState(IGameStateChanger stateChanger, AllServices services) 
        {
            _stateChanger = stateChanger;
            _saveLoadService = services.Single<SaveLoadService>();
            _windowService = services.Single<WindowService>();
            //_tutorialService = services.Single<TutorialService>();
            _services = services;
        }

        public void Enter()
        {
            _saveLoadService.LoadProgressAndInformReaders();
            SetState();

#if DEBUG
            _windowService.Open(WindowId.DEBUG);
#endif
        }


        private void SetState()
        {
            _stateChanger.Enter<Game_HubState>();
        }



        public void Exit()
        {

        }
    }
}