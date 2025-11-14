using System;
using Hexes;
using Infrastructure.Services.Core;
using Settings;
using UnityEngine;

namespace Input
{
    public class TapToChooseInput : IInputListener
    {
        private readonly CameraService _cameraService;
        private HexCell _currentHighlight;
        private LayerMask _layerMask;

        private Plane _plane;
        private SettingsStaticData _settings;

        public TapToChooseInput(CameraService cameraService, SettingsStaticData settings)
        {
            _settings = settings;
            _cameraService = cameraService;

            _plane = new Plane(Vector3.up, Vector3.zero);
        }

        public void OnClickDown()
        {
            var hit = RaycastBelowCursorWithOffset();

            OnTapDown?.Invoke(hit);
        }

        public void OnDrag()
        {
            var hit = RaycastBelowCursorWithOffset();

            OnDragTap?.Invoke(hit);
        }

        public void OnClickUp()
        {
            var hit = RaycastBelowCursorWithOffset();

            OnTapUp?.Invoke(hit);
        }

        public void OnCancelInput()
        {
        }

        public event Action<(Collider, Vector3)> OnTapDown;
        public event Action<(Collider, Vector3)> OnDragTap;
        public event Action<(Collider, Vector3)> OnTapUp;

        public void SetLayerMask(LayerMask mask)
        {
            _layerMask = mask;
        }

        private (Collider, Vector3) RaycastBelowCursorWithOffset()
        {
            var ray = GetMouseRay();

            _plane.Raycast(ray, out var distance);

            var point = ray.GetPoint(distance);

            var origin = point + _settings.TapToChooseOffset;

            if (Physics.Raycast(origin, Vector3.down, out var hitInfo,
                    _settings.RaycastDistance, _layerMask))
            {
                return (hitInfo.collider, origin);
            }

            return (null, origin);
        }

        private Ray GetMouseRay()
        {
            var camera = _cameraService.Camera;
            var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            return ray;
        }
    }
}