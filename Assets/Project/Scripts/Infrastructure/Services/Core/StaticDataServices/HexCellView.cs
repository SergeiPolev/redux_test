using DG.Tweening;
using Extensions;
using Lean.Pool;
using UnityEngine;

namespace Infrastructure.Services.Core
{
    public class HexCellView : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private MeshRenderer MeshRenderer;
        [SerializeField] private Material Highlighted;
        [SerializeField] private Material Normal;
        [SerializeField] private ParticleSystem PilePlacedVFX;

        public int x;
        public int y;

        #endregion

        private Tween _fillTween;
        private Tween _hoverTween;

        private Vector3 _initPos;

        private Tween _jumpTween;
        private Tween _pilePlacedTween;

        public int Index { get; private set; }

        public void Initialize(int index)
        {
            Index = index;
        }

        public void SetEnabled(bool value)
        {
            MeshRenderer.enabled = value;
        }

        /// <summary>
        ///     Changes material and disables collider
        /// </summary>
        /// <param name="value">state of highlight</param>
        public void SetHighlight(bool value)
        {
            MeshRenderer.material = value ? Highlighted : Normal;
        }

        public void OnHexFillAnimation()
        {
            _fillTween.KillTo0();
            _fillTween = transform.DOScaleY(-0.1f, 0.175f).SetLoops(2, LoopType.Yoyo).SetRelative();
        }

        public void OnHammeredAnimation()
        {
            _fillTween.KillTo0();
            _fillTween = transform.DOScaleY(-0.4f, 0.25f).SetLoops(2, LoopType.Yoyo).SetRelative();
        }

        public void OnPilePlacedAnimation()
        {
            var sequence = DOTween.Sequence();

            _initPos = transform.position;

            sequence.Append(transform.DOMoveY(0.5f, 0).SetRelative());
            sequence.Append(transform.DOMoveY(-0.5f, 0.15f).SetRelative());
            sequence.AppendCallback(PilePlacedVFX.Play);

            _pilePlacedTween = sequence.Play().SetLink(gameObject);
        }

        public void Hovered()
        {
            _hoverTween?.Kill();

            _hoverTween = transform.DOMoveY(0.4f, 0.175f).SetRelative().SetLink(gameObject);
        }

        public void Unhovered()
        {
            _hoverTween?.Kill();

            _hoverTween = transform.DOMove(_initPos, 0.175f).SetLink(gameObject);
        }

        public void Jump(float distanceDamping)
        {
            _jumpTween?.KillTo0();

            _jumpTween = transform.DOMoveY(1 * distanceDamping, 0.1f).SetRelative().SetLoops(2, LoopType.Yoyo)
                .SetLink(gameObject).SetEase(Ease.Linear);
        }

        public void Despawn()
        {
            LeanPool.Despawn(this);
        }
    }
}