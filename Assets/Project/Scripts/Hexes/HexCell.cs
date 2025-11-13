using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Infrastructure.Services.Core;
using Infrastructure.Services.Gameplay;
using UnityEngine;

namespace Hexes
{
    public class HexCell
    {
        public enum HexCellState
        {
            Normal,
            Disabled,
        }

        public readonly List<Hex> Hexes = new();


        public readonly int Index;
        public HexCellState CellState = HexCellState.Normal;
        public HexCellView ModelView;

        public Action<HexCell> OnCellChanged;
        public Action<int> OnHexesRemoved;
        public Vector3 WorldPos;
        public int x;
        public int y;

        public HexCell(int index, int x, int y)
        {
            Index = index;
            this.x = x;
            this.y = y;
        }

        public bool IsLocked => CellState != HexCellState.Normal;


        public void AddHexesInstant(ICollection<Hex> hexes)
        {
            Hexes.AddRange(hexes);

            foreach (var hex in hexes)
            {
                hex.HexModelView.transform.SetParent(ModelView.transform);
            }

            RebuildStack();

            ModelView.OnPilePlacedAnimation();
            OnCellChanged?.Invoke(this);
        }

        public async UniTask AddHexesAnimated(ICollection<Hex> hexes, CancellationToken cancellationToken = default)
        {
            var hadHexCount = Hexes.Count;
            Hexes.AddRange(hexes);

            for (var i = hadHexCount; i < Hexes.Count; i++)
            {
                var hex = Hexes[i];
                hex.HexModelView.transform.SetParent(ModelView.transform, true);
                var tween = hex.HexModelView.MoveToLocalPos(Vector3.up * (0.1f * i)).OnComplete(OnFillAnimation);

                await (i == Hexes.Count - 1
                    ? tween.WithCancellation(cancellationToken)
                    : tween.AsyncWaitForPosition(0.1f).AsUniTask().AttachExternalCancellation(cancellationToken));
            }

            OnCellChanged?.Invoke(this);
        }

        private void OnFillAnimation()
        {
            ModelView.OnHexFillAnimation();
        }

        public void AddHex(Hex hex)
        {
            Hexes.Add(hex);
        }

        public List<Hex> GetTopHexes()
        {
            var topHexes = new List<Hex>();
            var color = GetTopColor();

            for (var i = Hexes.Count - 1; i >= 0; i--)
            {
                var hex = Hexes[i];

                if (hex.ColorID == color)
                {
                    topHexes.Add(hex);
                    Hexes.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

            return topHexes;
        }

        public bool IsEmpty()
        {
            return !IsLocked && Hexes.Count == 0;
        }

        public bool IsOnlyColor(ColorID color)
        {
            foreach (var hex in Hexes)
            {
                if (hex.ColorID != color)
                {
                    return false;
                }
            }

            return true;
        }

        public ColorID GetTopColor()
        {
            return Hexes.Last().ColorID;
        }

        public int GetTopColorCount()
        {
            var color = GetTopColor();
            var count = 0;
            for (var i = Hexes.Count - 1; i >= 0; i--)
            {
                var hex = Hexes[i];
                if (hex.ColorID == color)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        public async UniTask RemoveHexes(CancellationToken cancellationToken)
        {
            var hexesCount = Hexes.Count;
            for (var index = hexesCount - 1; index >= 0; index--)
            {
                var hex = Hexes[index];
                var tween = hex.HexModelView.OnStackComplete();
                await tween.AsyncWaitForPosition(0.1f).AsUniTask().AttachExternalCancellation(cancellationToken);
            }

            Hexes.Clear();
            OnHexesRemoved?.Invoke(hexesCount);
            OnCellChanged?.Invoke(this);
        }

        public async UniTask RemoveTopHexes(CancellationToken cancellationToken)
        {
            var topHexes = GetTopHexes();
            var count = topHexes.Count;
            foreach (var hex in topHexes)
            {
                var tween = hex.HexModelView.OnStackComplete();
                await tween.AsyncWaitForPosition(0.1f).AsUniTask().AttachExternalCancellation(cancellationToken);
            }

            OnHexesRemoved?.Invoke(count);
            OnCellChanged?.Invoke(this);
        }

        public void SetState(HexCellState state)
        {
            CellState = state;
            ModelView.SetEnabled(state == HexCellState.Normal);
        }

        private void RebuildStack()
        {
            for (var i = 0; i < Hexes.Count; i++)
            {
                var hex = Hexes[i];

                hex.HexModelView.transform.localPosition = Vector3.up * (0.1f * i);
            }
        }

        public Hex PeekTopHex()
        {
            return Hexes.Last();
        }

        public void CleanUp()
        {
            for (var index = Hexes.Count - 1; index >= 0; index--)
            {
                var hex = Hexes[index];
                hex.HexModelView.Despawn();
            }

            ModelView.Despawn();
            ModelView = null;
            Hexes.Clear();
        }
    }
}