using Infrastructure.Services;
using Infrastructure.StateMachine;
using Infrastructure.States;
using UnityEngine;

namespace Infrastructure
{
    public class Game : MonoBehaviour, ICoroutineRunner
    {
        public GameStateMachine StateMachine { get; private set; }

        #region Event Functions

        private void Awake()
        {
            SetScreenSleep();
            SetStateMachine();
        }

        private void Update()
        {
            StateMachine.Tick();
        }

        private void FixedUpdate()
        {
            StateMachine.FixedTick();
        }

        private void OnApplicationQuit()
        {
            StateMachine.Enter<Game_AppQuit_State>();
        }

        #endregion

        private void SetScreenSleep()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void SetStateMachine()
        {
            StateMachine = new GameStateMachine(AllServices.Container, this, this);
            StateMachine.Enter<BootstrapState>();
        }
    }
}