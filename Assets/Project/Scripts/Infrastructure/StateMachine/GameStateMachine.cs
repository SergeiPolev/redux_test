using StateMachine;
using System.Collections.Generic;
using Services;
using System;

namespace Infrastructure
{
    public class GameStateMachine : StateMachineBase, IGameStateChanger
    {
        public GameStateMachine(AllServices services, ICoroutineRunner coroutineRunner)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(BootstrapState)] = new BootstrapState(this, services),
                [typeof(SelectGoogleSheetState)] = new SelectGoogleSheetState(this, services),
                [typeof(LoadGoogleSheetState)] = new LoadGoogleSheetState(this, services),
                [typeof(Game_LoadProgressState)] = new Game_LoadProgressState(this, services),
                [typeof(Game_HubState)] = new Game_HubState(this, services),
                [typeof(Game_LoadLevelState)] = new Game_LoadLevelState(this, services),
                [typeof(Game_LevelState)] = new Game_LevelState(this, services, coroutineRunner),
                [typeof(Game_ResultState)] = new Game_ResultState(this, services),
                [typeof(Game_CleanUpState)] = new Game_CleanUpState(this, services),
                [typeof(Game_AppQuit_State)] = new Game_AppQuit_State(this, services),
                
            };
        }
    }
}