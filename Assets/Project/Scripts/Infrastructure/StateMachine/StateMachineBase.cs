using System;
using System.Collections.Generic;

namespace Infrastructure.StateMachine
{
    public abstract class StateMachineBase
    {
        protected IExitableState _activeState;
        protected IFixedTick _fixedTickableState;
        protected ILateTick _lateTickableState;
        protected Dictionary<Type, IExitableState> _states;
        protected ITick _tickableState;

        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            IPayloadedState<TPayload> state = ChangeState<TState>();
            state.Enter(payload);
        }

        public void Tick()
        {
            _tickableState?.Tick();
        }

        public void FixedTick()
        {
            _fixedTickableState?.FixedTick();
        }

        public void LateTick()
        {
            _lateTickableState?.LateTick();
        }

        protected TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();
            var state = GetState<TState>();
            _activeState = state;

            if (_activeState is ITick tickableState)
            {
                _tickableState = tickableState;
            }
            else
            {
                _tickableState = null;
            }

            if (_activeState is IFixedTick fixedTickable)
            {
                _fixedTickableState = fixedTickable;
            }
            else
            {
                _fixedTickableState = null;
            }

            if (_activeState is ILateTick lateTickable)
            {
                _lateTickableState = lateTickable;
            }
            else
            {
                _lateTickableState = null;
            }

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState
        {
            return _states[typeof(TState)] as TState;
        }
    }
}