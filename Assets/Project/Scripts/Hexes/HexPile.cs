using System;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

namespace Infrastructure
{
    public class HexPile : MonoBehaviour
    {
        public Hex[] Hexes;
        public Vector3 OriginPos;
        
        private Tween _returnTween;

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
            for (int i = 0; i < Hexes.Length; i++)
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