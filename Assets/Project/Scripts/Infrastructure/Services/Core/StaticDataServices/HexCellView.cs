using DG.Tweening;
using Extensions;
using Lean.Pool;
using Settings;
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

        #endregion

        private Tween _fillTween;
        private Tween _hoverTween;

        private Vector3 _initPos;

        private Tween _jumpTween;
        private Tween _pilePlacedTween;
        private SettingsStaticData _settings;

        public int Index { get; private set; }

        public void Initialize(int index, SettingsStaticData settings)
        {
            _settings = settings;
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
            _fillTween = transform.DOScaleY(_settings.FillCellHeight, _settings.FillCellDuration).SetLoops(2, LoopType.Yoyo).SetRelative();
        }

        public void OnHammeredAnimation()
        {
            _fillTween.KillTo0();
            _fillTween = transform.DOScaleY(_settings.HammeredHeight, _settings.HammeredDuration).SetLoops(2, LoopType.Yoyo).SetRelative();
        }

        public void OnPilePlacedAnimation()
        {
            var sequence = DOTween.Sequence();

            _initPos = transform.position;

            sequence.Append(transform.DOMoveY(_settings.PilePlaceHeight, 0).SetRelative());
            sequence.Append(transform.DOMoveY(-_settings.PilePlaceHeight, _settings.PilePlaceDuration).SetRelative());
            sequence.AppendCallback(PilePlacedVFX.Play);

            _pilePlacedTween = sequence.Play().SetLink(gameObject);
        }

        public void Hovered()
        {
            _hoverTween?.Kill();

            _hoverTween = transform.DOMoveY(_settings.HoverHeight, _settings.HoverDuration).SetRelative().SetLink(gameObject);
        }

        public void Unhovered()
        {
            _hoverTween?.Kill();

            _hoverTween = transform.DOMove(_initPos, _settings.HoverDuration).SetLink(gameObject);
        }

        public void Jump(float distanceDamping)
        {
            _jumpTween?.KillTo0();

            _jumpTween = transform.DOMoveY(_settings.HammerJumpHeight * distanceDamping, _settings.HammerJumpDuration)
                .SetRelative()
                .SetLoops(2, LoopType.Yoyo)
                .SetLink(gameObject)
                .SetEase(Ease.Linear);
        }

        public void Despawn()
        {
            LeanPool.Despawn(this);
        }
    }
}