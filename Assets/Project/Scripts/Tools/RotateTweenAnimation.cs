using DG.Tweening;
using UnityEngine;

namespace Tools
{
    public class RotateTweenAnimation : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private float _duration = 1f;

        #endregion

        private Tween _rotateTween;

        #region Event Functions

        private void OnEnable()
        {
            StartRotate();
        }

        private void OnDisable()
        {
            StopRotate();
        }

        #endregion

        private void StartRotate()
        {
            transform.rotation = Quaternion.identity;
            _rotateTween = transform.DORotate(new Vector3(0, 0, 360), _duration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1)
                .SetUpdate(true);
        }

        private void StopRotate()
        {
            if (_rotateTween != null)
            {
                _rotateTween.Kill();
            }
        }
    }
}