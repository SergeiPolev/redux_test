using Hexes;
using Infrastructure.Services.Core;
using Infrastructure.Services.Gameplay;
using Layer_Manager;
using UnityEngine;

namespace Input
{
    public class DragAndDropPilesInput : IInputListener
    {
        private readonly CameraService _cameraService;
        private readonly IHexGridService _hexGridService;
        private HexCell _currentHighlight;
        private HexPile _draggableHexPile;

        private Plane _plane;

        public DragAndDropPilesInput(CameraService cameraService, IHexGridService hexGridService)
        {
            _cameraService = cameraService;
            _hexGridService = hexGridService;

            _plane = new Plane(Vector3.up, Vector3.zero);
        }

        private void HandleClickDown()
        {
            var ray = GetMouseRay();

            if (Physics.Raycast(ray.origin, ray.direction, out var hit, 100f, LayerManager.HexPileLayerMask))
            {
                if (hit.collider.TryGetComponent<HexPile>(out var hexPile))
                {
                    hexPile.OnPickUp();
                    _draggableHexPile = hexPile;
                }
            }
        }

        private void PlaceDraggable()
        {
            var cell = _currentHighlight;

            if (cell == null || !cell.IsEmpty())
            {
                _draggableHexPile.MoveToOriginPos();
                _draggableHexPile = null;

                return;
            }

            cell.ModelView.SetHighlight(false);
            cell.AddHexesInstant(_draggableHexPile.Hexes);
            _draggableHexPile.CleanUp();

            UnselectCell();

            _draggableHexPile = null;
        }

        private void HandleDrag()
        {
            if (_draggableHexPile == null)
            {
                return;
            }

            var hit = RaycastBelowCursor();

            _draggableHexPile.transform.position = hit + new Vector3(0, 0.5f, 0.5f);

            UnselectCell();

            var cell = _hexGridService.GetClosestCell(_draggableHexPile.transform.position);

            if (cell == null || !cell.IsEmpty())
            {
                return;
            }

            cell.ModelView.SetHighlight(true);
            _currentHighlight = cell;
        }

        private Vector3 RaycastBelowCursor()
        {
            var ray = GetMouseRay();

            _plane.Raycast(ray, out var distance);

            return ray.GetPoint(distance);
        }

        private Ray GetMouseRay()
        {
            var camera = _cameraService.Camera;
            var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            return ray;
        }

        private void UnselectCell()
        {
            if (_currentHighlight != null)
            {
                _currentHighlight.ModelView.SetHighlight(false);
                _currentHighlight = null;
            }
        }

        #region IInputListener Members

        public void OnClickDown()
        {
            HandleClickDown();
        }

        public void OnDrag()
        {
            HandleDrag();
        }

        public void OnClickUp()
        {
            if (_draggableHexPile != null)
            {
                PlaceDraggable();
            }
        }

        public void OnCancelInput()
        {
            UnselectCell();
        }

        #endregion
    }
}