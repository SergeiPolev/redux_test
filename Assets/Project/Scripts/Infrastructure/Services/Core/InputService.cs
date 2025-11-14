using System;
using Infrastructure.Services.Gameplay;
using Infrastructure.StateMachine;
using Input;
using UnityEngine;

namespace Infrastructure.Services.Core
{
    public enum InputType
    {
        DragAndDrop,
        Idle,
        TapToChoose,
    }

    public enum ForcedInputType
    {
        None,
        Forced,
    }

    public class InputService : IService, ILateTick
    {
        private IInputListener _currentListener;

        private IInputListener _dragAndDropListener;
        private ForcedInputType _forcedInputType = ForcedInputType.None;
        private IInputListener _forcedListener;
        private IInputListener _idleListener;

        private InputType _inputType;
        private IInputListener _tapToChooseListener;

        public T GetCurrentListener<T>(InputType inputType) where T : class, IInputListener
        {
            if (inputType == _inputType)
            {
                return _currentListener as T;
            }

            Debug.LogError($"Input type {inputType} is not current! Returning not active listener!");
            return GetListener(_inputType) as T;
        }

        public void Initialize(CameraService cameraService, IHexGridService hexGridService, GlobalBlackboard blackboard)
        {
            _dragAndDropListener = new DragAndDropPilesInput(cameraService, hexGridService, blackboard.Settings);
            _idleListener = new IdleInput();
            _tapToChooseListener = new TapToChooseInput(cameraService, blackboard.Settings);

            _currentListener = _dragAndDropListener;
        }

        public void LateTick()
        {
            if (_forcedInputType == ForcedInputType.None)
            {
                StandardInputTick();
            }
            else
            {
                ForcedInputTick();
            }
        }

        private void ForcedInputTick()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                _forcedListener.OnClickDown();
            }
            else if (UnityEngine.Input.GetMouseButton(0))
            {
                _forcedListener.OnDrag();
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                _forcedListener.OnClickUp();
            }
        }

        private void StandardInputTick()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                _currentListener.OnClickDown();
            }
            else if (UnityEngine.Input.GetMouseButton(0))
            {
                _currentListener.OnDrag();
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                _currentListener.OnClickUp();
            }
        }

        public IInputListener SetListenerType(InputType inputType)
        {
            _currentListener.OnCancelInput();

            _inputType = inputType;

            _currentListener = GetListener(inputType);

            return _currentListener;
        }

        public IInputListener SetForcedInputType(InputType inputType)
        {
            _forcedListener = GetListener(inputType);
            _forcedInputType = ForcedInputType.Forced;

            return _forcedListener;
        }

        public void ResetForcedInputType()
        {
            _forcedInputType = ForcedInputType.None;
        }

        private IInputListener GetListener(InputType inputType)
        {
            return inputType switch
            {
                InputType.DragAndDrop => _dragAndDropListener,
                InputType.Idle => _idleListener,
                InputType.TapToChoose => _tapToChooseListener,
                _ => throw new ArgumentOutOfRangeException(nameof(inputType), inputType, null),
            };
        }
    }
}