using Services;
using StateMachine;

namespace Infrastructure
{
    public class Level_NextStageState : IState, ITick
    {
        private IStateChanger _sessionStateMachine;
        private WindowService _windowService;
        private LevelProgressService _levelProgressService;
        private ResultService _resultService;

        public Level_NextStageState(IStateChanger sessionStateMachine, AllServices services)
        {
            _sessionStateMachine = sessionStateMachine;
            _windowService = services.Single<WindowService>();
            _levelProgressService = services.Single<LevelProgressService>();
            _resultService = services.Single<ResultService>();
        }

        public void Enter()
        {
            _sessionStateMachine.Enter<Level_StartLevelState>();

            // Condition for win
            if (true)
            {
                Win();
            }
            else
            {
                NextStage();
            }
        }

        private void NextStage()
        {
            AllServices.Container.Single<SDKService>().Level_Step_Event();
            
            _sessionStateMachine.Enter<Level_StartLevelState>();
        }

        private void Win()
        {
            
        }

        private void SetStateWin()
        {
            _resultService.Win();
        }

        public void Exit()
        {
            
        }

        public void Tick()
        {
            
        }
    }
}