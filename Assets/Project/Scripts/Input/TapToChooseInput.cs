using System;
using Services;
using UnityEngine;

namespace Infrastructure
{
    public class TapToChooseInput : IInputListener
    {
        private CameraService _cameraService;
        private LayerMask _layerMask;

        private Plane _plane;
        private HexCell _currentHighlight;
        private Vector3 _offset = new Vector3(0, 0, 0.25f);

        public event Action<(Collider, Vector3)> OnTapDown;
        public event Action<(Collider, Vector3)> OnDragTap;
        public event Action<(Collider, Vector3)> OnTapUp;

        public TapToChooseInput(CameraService cameraService)
        {
            _cameraService = cameraService;

            _plane = new Plane(Vector3.up, Vector3.zero);
        }

        public void SetLayerMask(LayerMask mask)
        {
            _layerMask = mask;
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
        
        private (Collider, Vector3) RaycastBelowCursorWithOffset()
        {
            var ray = GetMouseRay();

            _plane.Raycast(ray, out float distance);

            Vector3 point = ray.GetPoint(distance);
            
            Vector3 origin = point + _offset;
            
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hitInfo,
                    float.MaxValue, _layerMask))
            {
                return (hitInfo.collider, origin);
            }
            
            return (null, origin);
        }
        
        private Ray GetMouseRay()
        {
            Camera camera = _cameraService.Camera;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            return ray;
        }
    }
}