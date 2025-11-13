using System;
using Boosters.Views;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Extensions;
using Factories.GameFactory;
using Hexes;
using Infrastructure.Services;
using Infrastructure.Services.Core;
using Infrastructure.Services.Gameplay;
using Input;
using Layer_Manager;
using Lean.Pool;
using UnityEngine;

namespace Boosters
{
    public class BreakPilesBooster : IBooster
    {
        private readonly CancellationAsyncService _cancellationAsyncService;
        private readonly GameFactory _gameFactory;
        private readonly IHexGridService _hexService;
        private readonly InputService _inputService;
        private HammerView _hammer;
        private HexCell _lastHoveredHexCell;

        private TapToChooseInput _tapToChooseInput;

        private float _timer;

        public BreakPilesBooster(AllServices services)
        {
            _hexService = services.Single<IHexGridService>();
            _inputService = services.Single<InputService>();
            _gameFactory = services.Single<GameFactory>();
            _cancellationAsyncService = services.Single<CancellationAsyncService>();
        }

        public bool IsActive { get; private set; }

        public event Action<BoosterProgressEvent> OnProgressChanged;

        public event Action OnActivated;
        public event Action OnDeactivated;

        public BoosterId BoosterId => BoosterId.PaintUpperHexes;

        public async void Activate()
        {
            IsActive = true;

            _timer = 0;
            _hammer = _gameFactory.CreateHammerView();

            _inputService.SetForcedInputType(InputType.Idle);

            await _hammer.Appear().AsyncWaitForCompletion().AsUniTask()
                .AttachExternalCancellation(_cancellationAsyncService.Token);

            _tapToChooseInput = _inputService.SetForcedInputType(InputType.TapToChoose) as TapToChooseInput;
            _tapToChooseInput.SetLayerMask(LayerManager.CellLayer);

            _tapToChooseInput.OnDragTap += DragBrush;
            _tapToChooseInput.OnTapUp += ChoosePile;

            OnActivated?.Invoke();
        }

        public void Tick()
        {
            if (!IsActive)
            {
                return;
            }

            OnProgressChanged?.Invoke(new BoosterProgressEvent(_timer, 1, 1 - _timer));
        }

        public void Deactivate()
        {
            IsActive = false;

            LeanPool.Despawn(_hammer);
            _inputService.ResetForcedInputType();

            OnDeactivated?.Invoke();
        }

        private void DragBrush((Collider, Vector3) obj)
        {
            var hexCell = _hexService.GetClosestCell(obj.Item2);

            if (hexCell is { IsLocked: false } && !hexCell.IsEmpty())
            {
                if (_lastHoveredHexCell != null)
                {
                    if (hexCell.Index == _lastHoveredHexCell.Index)
                    {
                        return;
                    }

                    _lastHoveredHexCell.ModelView.Unhovered();
                }

                _lastHoveredHexCell = hexCell;
                _lastHoveredHexCell.ModelView.Hovered();

                _hammer.transform.position = _lastHoveredHexCell.WorldPos + Vector3.up;

                return;
            }

            if (_lastHoveredHexCell != null)
            {
                _lastHoveredHexCell.ModelView.Unhovered();
                _lastHoveredHexCell = null;
            }

            _hammer.transform.position = obj.Item2 + Vector3.up;
        }

        private async void ChoosePile((Collider, Vector3) obj)
        {
            var hexCell = _hexService.GetClosestCell(obj.Item2);

            if (hexCell is { IsLocked: false } && !hexCell.IsEmpty())
            {
                _inputService.SetForcedInputType(InputType.Idle);

                _tapToChooseInput.OnDragTap -= DragBrush;
                _tapToChooseInput.OnTapUp -= ChoosePile;

                _lastHoveredHexCell?.ModelView.Unhovered();

                await _hammer.Punch().ToUniTask(_cancellationAsyncService.Token);

                hexCell.ModelView.OnHammeredAnimation();
                _hammer.Disappear();

                SpreadHitWave(hexCell.x, hexCell.y);

                await UniTask.WaitForSeconds(0.5f, cancellationToken: _cancellationAsyncService.Token);

                await hexCell.RemoveHexes(_cancellationAsyncService.Token);

                IsActive = false;
            }
        }

        private async void SpreadHitWave(int x, int y)
        {
            var grid = _hexService.GetGrid();

            float waves = Mathf.Max(grid.Width, grid.Height);

            for (var i = 1; i < waves; i++)
            {
                var cells = grid.GetRing(x, y, i);

                foreach (var cell in cells)
                {
                    cell.ModelView.Jump(1 - (i - 1) / waves);
                }

                await UniTask.WaitForSeconds(0.075f, cancellationToken: _cancellationAsyncService.Token);
            }
        }
    }
}