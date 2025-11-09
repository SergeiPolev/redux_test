using StateMachine;
using System.Collections.Generic;
using Services;
using System;

namespace Infrastructure
{
    public class LevelStateMachine : StateMachineBase, IStateChanger
    {
        public LevelStateMachine(IGameStateChanger gameStateChanger, AllServices services,
            ICoroutineRunner coroutineRunner)
        {
            _states = new Dictionary<Type, IExitableState>()
            {
                [typeof(Level_InitLevelState)] = new Level_InitLevelState(this, gameStateChanger, services),
                [typeof(Level_StartLevelState)] = new Level_StartLevelState(this, gameStateChanger, services),
                [typeof(Level_LevelState)] = new Level_LevelState(this, gameStateChanger, services),
                [typeof(Level_NextStageState)] = new Level_NextStageState(this, services),
                [typeof(Level_CleanUpState)] = new Level_CleanUpState(this, gameStateChanger, services),
            };
        }
    }
}