using Factories.GameFactory;
using Factories.UIFactory;
using Infrastructure.Services;
using Infrastructure.Services.Core;
using Infrastructure.Services.Gameplay;
using Infrastructure.Services.Progress;
using Infrastructure.StateMachine;
using UnityEngine;

namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
        private readonly MonoBehaviour _monoBehaviour;
        private readonly AllServices _services;
        private readonly IGameStateChanger _stateChanger;

        public BootstrapState(IGameStateChanger stateChanger, AllServices services, MonoBehaviour monoBehaviour)
        {
            _monoBehaviour = monoBehaviour;
            _stateChanger = stateChanger;
            _services = services;

            RegisterServices();
            InitServices();
        }

        public void Enter()
        {
            _stateChanger.Enter<Game_InitializeState>();
        }


        public void Exit()
        {
        }

        private void RegisterServices()
        {
            // Core
            _services.RegisterSingle(new CancellationAsyncService());
            _services.RegisterSingle(new StaticDataService());
            _services.RegisterSingle(new GlobalBlackboard());
            _services.RegisterSingle(new InputService());
            _services.RegisterSingle(new GameFactory());
            _services.RegisterSingle(new UIFactory());
            _services.RegisterSingle(new WindowService());
            _services.RegisterSingle(new CameraService());

            // Progress
            _services.RegisterSingle(new LevelProgressService());

            // Gameplay
            _services.RegisterSingle(new ColorMaterialsService());
            _services.RegisterSingle(new HexPilesService());
            _services.RegisterSingle<IHexGridService>(new HexGridService());
            _services.RegisterSingle(new MergeService());
            _services.RegisterSingle(new ClearStackService());
            _services.RegisterSingle(new BoosterService());
            _services.RegisterSingle(new ResultService());
        }

        private void InitServices()
        {
            InitCore();
            InitProgress();
            InitGameplay();
        }

        private void InitProgress()
        {
            _services.Single<LevelProgressService>().Initialize(_services);
        }

        private void InitGameplay()
        {
            _services.Single<ColorMaterialsService>().Initialize(_services.Single<StaticDataService>(),
                _services.Single<GlobalBlackboard>());
            
            _services.Single<IHexGridService>().Initialize(_services);
            _services.Single<HexPilesService>().Initialize(
                _services.Single<CameraService>(),
                _services.Single<GameFactory>(),
                _services.Single<InputService>(),
                _services.Single<CancellationAsyncService>(),
                _services.Single<LevelProgressService>());
            
            _services.Single<ClearStackService>().Initialize(
                _services.Single<IHexGridService>(),
                _services.Single<MergeService>(),
                _services.Single<CancellationAsyncService>(),
                _services.Single<LevelProgressService>());
            
            _services.Single<MergeService>().Initialize(_services.Single<IHexGridService>(),
                _services.Single<CancellationAsyncService>());
            
            _services.Single<BoosterService>().Initialize(_services);
            
            _services.Single<ResultService>().Initialize(_stateChanger, _services.Single<LevelProgressService>(),
                _services.Single<MergeService>(), _services.Single<IHexGridService>());
        }

        private void InitCore()
        {
            _services.Single<StaticDataService>().Initialize();
            _services.Single<GlobalBlackboard>().Initialize(_services.Single<StaticDataService>());

            _services.Single<InputService>()
                .Initialize(_services.Single<CameraService>(), _services.Single<IHexGridService>(), _services.Single<GlobalBlackboard>());

            _services.Single<WindowService>().Initialize(_services.Single<UIFactory>());
            _services.Single<UIFactory>().Initialize(_services);
            _services.Single<GameFactory>()
                .Initialize(_services.Single<GlobalBlackboard>(), _services.Single<StaticDataService>(),
                    _services.Single<CameraService>());

            _services.Single<CancellationAsyncService>().Initialize(_monoBehaviour);
            _services.Single<CameraService>().Initialize(_services.Single<IHexGridService>());
        }
    }
}