using StateMachine;
using Services;
using static UnityEngine.CullingGroup;

namespace Infrastructure
{
    public class Level_StartLevelState : IState
    {
        private IStateChanger _stateChanger;
        private WindowService _windowService;
        private CameraService _cameraService;
        private LevelProgressService _progress;
        private SDKService _sdkService;

        public Level_StartLevelState(IStateChanger stateChanger, IGameStateChanger gameStateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _windowService = services.Single<WindowService>();
            _cameraService = services.Single<CameraService>();
            _progress = services.Single<LevelProgressService>();
            _sdkService = services.Single<SDKService>();
        }
        public void Enter()
        {
            _progress.Stage_Start();
            _sdkService.Level_Start_Event();
            _stateChanger.Enter<Level_LevelState>();
        }

        public void Exit()
        {

        }
    }
}