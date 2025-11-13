using Infrastructure.Services;
using Infrastructure.Services.Progress;
using Infrastructure.StateMachine;

namespace Infrastructure
{
    public class Level_StartLevelState : IState
    {
        private readonly LevelProgressService _progress;
        private readonly IStateChanger _stateChanger;

        public Level_StartLevelState(IStateChanger stateChanger, IGameStateChanger gameStateChanger,
            AllServices services)
        {
            _stateChanger = stateChanger;
            _progress = services.Single<LevelProgressService>();
        }

        public void Enter()
        {
            _progress.Stage_Start();
            _stateChanger.Enter<Level_LevelState>();
        }

        public void Exit()
        {
        }
    }
}