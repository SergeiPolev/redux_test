using Services;
using IState = StateMachine.IState;

namespace Infrastructure
{
    internal class Game_LevelState : IState, ITick, IFixedTick, ILateTick
    {
        private IGameStateChanger _stateChanger;
        
        private LevelStateMachine _lvlStageMachine;

        public Game_LevelState(IGameStateChanger stateChanger, AllServices services, ICoroutineRunner coroutineRunner)
        {
            _stateChanger = stateChanger;

            _lvlStageMachine = new LevelStateMachine(stateChanger, services, coroutineRunner);
            services.Single<ResultService>().SetLevelStateChanger(_lvlStageMachine);
        }

        public void Enter()
        {
            _lvlStageMachine.Enter<Level_InitLevelState>();
        }
        public void Exit()
        {
            _lvlStageMachine.Enter<Level_CleanUpState>();            
        }
        
        public void Tick()
        {
            _lvlStageMachine.Tick();
        }

        public void FixedTick()
        {
            _lvlStageMachine.FixedTick();
        }

        public void LateTick()
        {
            _lvlStageMachine.LateTick();
        }
    }
}