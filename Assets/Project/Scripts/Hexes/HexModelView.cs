using DG.Tweening;
using Lean.Pool;
using UnityEngine;

namespace Hexes
{
    public class HexModelView : MonoBehaviour
    {
        private static readonly int _colorCached = Shader.PropertyToID("_BaseColor");

        #region Serialized Fields

        [SerializeField] private MeshRenderer _modelRenderer;
        [SerializeField] private ParticleSystem _landVFX;

        #endregion

        private Vector3 _originalRotation;

        #region Event Functions

        private void Start()
        {
            _originalRotation = _modelRenderer.transform.localRotation.eulerAngles;
        }

        #endregion

        public void Initialize()
        {
            _modelRenderer.transform.localRotation = Quaternion.Euler(_originalRotation);
        }

        public void SetMaterial(Material material)
        {
            _modelRenderer.material = material;
        }

        public void SetColor(Color color)
        {
            var materialPropertyBlock = new MaterialPropertyBlock();
            _modelRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor(_colorCached, color);
            _modelRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        public void SetVFXColor(Color color)
        {
            var module = _landVFX.main;

            module.startColor = color;
        }

        public Tween MoveToLocalPos(Vector3 position)
        {
            var tween = DOTween.Sequence();

            var destination = position;
            destination.y = transform.localPosition.y;
            _modelRenderer.transform.forward = destination - transform.localPosition;

            var duration = 0.4f;

            tween.Append(transform
                .DOLocalJump(position, Mathf.Max(1, transform.localPosition.y - position.y), 1, duration)
                .SetEase(Ease.Linear));
            tween.Join(_modelRenderer.transform.DOLocalRotate(Vector3.right * 180, duration, RotateMode.LocalAxisAdd)
                .SetRelative());
            tween.AppendCallback(_landVFX.Play);

            return tween.SetLink(gameObject);
        }

        public Tween OnStackComplete()
        {
            var tween = DOTween.Sequence();

            var duration = 0.4f;

            tween.AppendCallback(_landVFX.Play);

            tween.Append(transform.DOLocalMoveY(2, duration));
            tween.Join(_modelRenderer.transform.DOLocalRotate(Vector3.up * 90, duration, RotateMode.LocalAxisAdd)
                .SetRelative());
            tween.Join(transform.DOScale(0, duration));
            tween.AppendCallback(Despawn);

            return tween.SetLink(gameObject);
        }

        public void Despawn()
        {
            LeanPool.Despawn(gameObject);
        }

        public Tween LerpColor(Material material, float duration)
        {
            return DOVirtual.Float(0, 1, duration,
                x => _modelRenderer.material.Lerp(_modelRenderer.material, material, x));
        }
    }
}