using System;
using System.Linq;
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
    public class PaintUpperHexesBooster : IBooster
    {
        private readonly CancellationAsyncService _cancellationAsyncService;
        private readonly GameFactory _gameFactory;
        private readonly IHexGridService _hexService;
        private readonly InputService _inputService;
        private BrushView _brush;
        private HexCell _lastHoveredHexCell;

        private TapToChooseInput _tapToChooseInput;

        private float _timer;

        public PaintUpperHexesBooster(AllServices services)
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
            _brush = _gameFactory.CreateBrushView();

            _inputService.SetForcedInputType(InputType.Idle);

            await _brush.Appear().AsyncWaitForCompletion().AsUniTask()
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

            LeanPool.Despawn(_brush);
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

                var material = _gameFactory.GetColorMaterial(hexCell.GetTopColor());
                _brush.SetInkMaterial(material);

                _brush.transform.position = hexCell.PeekTopHex().HexModelView.transform.position + Vector3.up;

                return;
            }

            if (_lastHoveredHexCell != null)
            {
                _lastHoveredHexCell.ModelView.Unhovered();
                _lastHoveredHexCell = null;
            }

            _brush.transform.position = obj.Item2 + Vector3.up;
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

                var colorID = hexCell.GetTopColor();

                var neighbors = _hexService.GetNeighbors(hexCell.x, hexCell.y).Where(x => !x.IsEmpty()).ToList();
                var material = _gameFactory.GetColorMaterial(colorID);

                _brush.SetInkMaterial(material);
                await _brush.BeginColoring(hexCell.WorldPos, _gameFactory.GetColor(colorID))
                    .ToUniTask(_cancellationAsyncService.Token);

                var endColoringTween = _brush.EndColoring();

                if (neighbors.Count > 0)
                {
                    for (var index = 0; index < neighbors.Count; index++)
                    {
                        var neighbor = neighbors[index];
                        var hex = neighbor.PeekTopHex();
                        var tween = hex.HexModelView.LerpColor(material, endColoringTween.Duration() - 0.1f * index);
                        hex.ColorID = colorID;

                        if (neighbors.Count - 1 == index)
                        {
                            await tween.AsyncWaitForCompletion().AsUniTask()
                                .AttachExternalCancellation(_cancellationAsyncService.Token);
                        }
                        else
                        {
                            await UniTask.WaitForSeconds(0.1f, cancellationToken: _cancellationAsyncService.Token);
                        }
                    }
                }
                else
                {
                    await endColoringTween.ToUniTask(_cancellationAsyncService.Token);
                }

                hexCell.OnCellChanged?.Invoke(hexCell);
                IsActive = false;
            }
        }
    }
}