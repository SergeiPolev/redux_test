using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Base
{
    public class ProgressBar : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _amountText;

        #endregion

        private bool _initialized;

        private float _originalDelta;

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _originalDelta = _fillImage.rectTransform.rect.width;
            _initialized = true;
        }

        public void UpdateValue(float current, float goal)
        {
            var value = current / goal;

            var rectSize = _fillImage.rectTransform.sizeDelta;
            rectSize.x = Mathf.Clamp(_originalDelta * (1 - value) * -1, -_originalDelta, 0);
            _fillImage.rectTransform.sizeDelta = rectSize;
            _amountText.SetText($"{current}/{goal}");
        }
    }
}