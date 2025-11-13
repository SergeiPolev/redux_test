using DG.Tweening;
using UnityEngine;

namespace Boosters.Views
{
    public class HammerView : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private ParticleSystem _vfx;

        #endregion

        public Tween Appear()
        {
            _meshRenderer.transform.DOScale(0, 0);
            transform.rotation = Quaternion.identity;

            return _meshRenderer.transform.DOScale(1, 0.25f).SetLink(gameObject);
        }

        public Tween Punch()
        {
            var sequence = DOTween.Sequence();

            sequence.Append(
                _meshRenderer.transform.DOLocalRotate(new Vector3(0, 0, 90), 0.35f, RotateMode.LocalAxisAdd));
            sequence.Append(
                _meshRenderer.transform.DOLocalRotate(new Vector3(0, 0, -100), 0.1f, RotateMode.LocalAxisAdd));
            sequence.AppendCallback(_vfx.Play);

            return sequence.Play().SetLink(gameObject).SetEase(Ease.Linear);
        }

        public void Disappear()
        {
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(0.5f);
            sequence.Append(
                _meshRenderer.transform.DOLocalRotate(new Vector3(0, 0, 10), 0.5f, RotateMode.LocalAxisAdd));
            sequence.Append(_meshRenderer.transform.DOScale(0, 0.5f));
            sequence.SetLink(gameObject);
        }
    }
}