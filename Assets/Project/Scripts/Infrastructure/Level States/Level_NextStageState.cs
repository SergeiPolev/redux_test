using Infrastructure.Services;
using Infrastructure.Services.Core;
using Infrastructure.Services.Progress;
using Infrastructure.StateMachine;

namespace Infrastructure
{
    public class Level_NextStageState : IState, ITick
    {
        private readonly IStateChanger _sessionStateMachine;

        public Level_NextStageState(IStateChanger sessionStateMachine, AllServices services)
        {
            _sessionStateMachine = sessionStateMachine;
            services.Single<WindowService>();
            services.Single<LevelProgressService>();
        }

        public void Enter()
        {
            _sessionStateMachine.Enter<Level_StartLevelState>();
        }

        public void Exit()
        {
        }

        public void Tick()
        {
        }
    }
}