using System;
using DG.Tweening;
using Extensions;
using Lean.Pool;
using UnityEngine;

namespace Hexes
{
    public class HexPile : MonoBehaviour
    {
        #region Serialized Fields

        public Vector3 OriginPos;

        #endregion

        private Tween _returnTween;
        public Hex[] Hexes;

        public event Action<HexPile> OnPileRemoved;

        public void OnPickUp()
        {
            _returnTween.KillTo0();
        }

        public void CleanUp()
        {
            LeanPool.Despawn(this);
            OnPileRemoved?.Invoke(this);
        }

        public void RebuildStack()
        {
            for (var i = 0; i < Hexes.Length; i++)
            {
                var hex = Hexes[i];

                hex.HexModelView.transform.localPosition = Vector3.up * (0.1f * i);
            }
        }

        public void MoveToOriginPos()
        {
            _returnTween = transform.DOMove(OriginPos, 0.2f).SetLink(gameObject);
        }
    }
}