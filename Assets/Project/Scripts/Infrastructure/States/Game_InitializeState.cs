using Infrastructure.Services;
using Infrastructure.Services.Gameplay;
using Infrastructure.StateMachine;

namespace Infrastructure.States
{
    internal class Game_InitializeState : IState
    {
        private readonly ColorMaterialsService _colorMaterialsService;
        private readonly IGameStateChanger _stateChanger;

        public Game_InitializeState(IGameStateChanger stateChanger, AllServices services)
        {
            _stateChanger = stateChanger;
            _colorMaterialsService = services.Single<ColorMaterialsService>();
        }

        public void Enter()
        {
            _colorMaterialsService.UpdateColors();

            SetState();
        }

        public void Exit()
        {
        }

        private void SetState()
        {
            _stateChanger.Enter<Game_HubState>();
        }
    }
}