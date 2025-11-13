using Cysharp.Threading.Tasks;
using Infrastructure.Services;
using Infrastructure.Services.Core;
using Infrastructure.Services.Gameplay;
using Infrastructure.Services.Progress;
using Infrastructure.StateMachine;
using UI;
using UnityEngine;

namespace Infrastructure
{
    public class Level_InitLevelState : IState
    {
        private readonly CameraService _cameraService;
        private readonly CancellationAsyncService _cancellationAsyncService;
        private readonly IHexGridService _hexGridService;
        private readonly LevelProgressService _levelProgress;
        private readonly ResultService _resultService;
        private readonly IStateChanger _stateChanger;
        private readonly WindowService _windowService;

        public Level_InitLevelState(IStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _windowService = services.Single<WindowService>();
            _levelProgress = services.Single<LevelProgressService>();
            _hexGridService = services.Single<IHexGridService>();
            _cameraService = services.Single<CameraService>();
            _cancellationAsyncService = services.Single<CancellationAsyncService>();
            _resultService = services.Single<ResultService>();
        }

        public async void Enter()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

            _levelProgress.LevelStarted();
            _windowService.Open(WindowId.LoadingScreen);

            _hexGridService.CreateGrid();
            _cameraService.SetCenterPos();
            _resultService.OnLevelEnter();

            await UniTask.WaitForEndOfFrame(_cancellationAsyncService.Token);

            _stateChanger.Enter<Level_StartLevelState>();
        }

        public void Exit()
        {
            _windowService.Close(WindowId.LoadingScreen);
        }
    }
}