using System;
using System.Collections.Generic;
using Infrastructure.Services;
using Infrastructure.States;
using UnityEngine;

namespace Infrastructure.StateMachine
{
    public class GameStateMachine : StateMachineBase, IGameStateChanger
    {
        public GameStateMachine(AllServices services, ICoroutineRunner coroutineRunner, MonoBehaviour monoBehaviour)
        {
            _states = new Dictionary<Type, IExitableState>
            {
                [typeof(BootstrapState)] = new BootstrapState(this, services, monoBehaviour),
                [typeof(Game_InitializeState)] = new Game_InitializeState(this, services),
                [typeof(Game_HubState)] = new Game_HubState(this, services),
                [typeof(Game_LoadLevelState)] = new Game_LoadLevelState(this, services),
                [typeof(Game_LevelState)] = new Game_LevelState(this, services),
                [typeof(Game_ResultState)] = new Game_ResultState(this, services),
                [typeof(Game_CleanUpState)] = new Game_CleanUpState(this, services),
                [typeof(Game_AppQuit_State)] = new Game_AppQuit_State(this, services),
            };
        }
    }
}