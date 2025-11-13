using StateMachine;
using Services;
using UnityEngine;

namespace Infrastructure
{
    public class BootstrapState : IState
    {
        private IGameStateChanger _stateChanger;
        private AllServices _services;
        private MonoBehaviour _monoBehaviour;

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

        private void RegisterServices()
        {
            _services.RegisterSingle(new StaticDataService());
            _services.RegisterSingle(new SaveLoadService());
            _services.RegisterSingle(new LevelProgressService());
            _services.RegisterSingle(new WalletService());
            _services.RegisterSingle(new GameWalletService());

            _services.RegisterSingle(new UIFactory());
            _services.RegisterSingle(new WindowService());

            _services.RegisterSingle(new GlobalBlackboard());
            _services.RegisterSingle(new InputService());
            _services.RegisterSingle(new HexPilesService());
            _services.RegisterSingle(new MergeService());
            _services.RegisterSingle(new ClearStackService());
            _services.RegisterSingle(new CancellationAsyncService());
            _services.RegisterSingle(new BoosterService());

            _services.RegisterSingle(new GameFactory());
            _services.RegisterSingle(new CameraService());
            _services.RegisterSingle(new ResultService());
            _services.RegisterSingle(new ColorMaterialsService());
            
            _services.RegisterSingle<IHexGridService>(new HexGridService());
        }

        private void InitServices()
        {
            InitCore();
            
            _services.Single<WalletService>().Initialize();
            _services.Single<GameWalletService>().Initialize();
            _services.Single<LevelProgressService>().Initialize(_services);
            _services.Single<GlobalBlackboard>().Initialize();
            _services.Single<BoosterService>().Initialize(_services);
            _services.Single<CancellationAsyncService>().Initialize(_monoBehaviour);
            _services.Single<ResultService>().Initialize(_stateChanger, _services.Single<LevelProgressService>(),  _services.Single<MergeService>(),  _services.Single<IHexGridService>());
            _services.Single<IHexGridService>().Initialize(_services);
            _services.Single<HexPilesService>().Initialize(
                _services.Single<CameraService>(),
                _services.Single<GameFactory>(),
                _services.Single<InputService>(),
                _services.Single<CancellationAsyncService>(),
                _services.Single<LevelProgressService>());
            _services.Single<InputService>().Initialize(_services.Single<CameraService>(), _services.Single<IHexGridService>());
            _services.Single<MergeService>().Initialize(_services.Single<IHexGridService>(), _services.Single<CancellationAsyncService>());
            
            _services.Single<ClearStackService>().Initialize(
                _services.Single<IHexGridService>(),
                _services.Single<MergeService>(),
                _services.Single<CancellationAsyncService>(),
                _services.Single<LevelProgressService>());

            _services.Single<CameraService>().Initialize(_services.Single<IHexGridService>());
            _services.Single<GameFactory>()
                .Initialize(_services.Single<GlobalBlackboard>(), _services.Single<StaticDataService>(), _services.Single<CameraService>());


            _services.Single<ColorMaterialsService>().Initialize(_services.Single<StaticDataService>(), _services.Single<GlobalBlackboard>());
        }

        private void InitCore()
        {
            _services.Single<StaticDataService>().Initialize();
            _services.Single<UIFactory>().Initialize(_services);
            _services.Single<SaveLoadService>().Initialize(_services);
            _services.Single<WindowService>().Initialize(_services.Single<UIFactory>());
            
        }


        public void Exit()
        {

        }
    }
}