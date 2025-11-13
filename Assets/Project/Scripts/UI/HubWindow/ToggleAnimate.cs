using UnityEngine;
using UnityEngine.UI;

namespace UI.HubWindow
{
    public class ToggleAnimate : MonoBehaviour
    {
        private readonly int _isOnHash = Animator.StringToHash("IsOn");
        private readonly int _isToggleOffAnimationHash = Animator.StringToHash("ToggleOff");
        private readonly int _isToggleOnAnimationHash = Animator.StringToHash("ToggleOn");
        private Animator _animator;
        private CustomToggle _hubNavigationToggle;
        private Toggle _toggle;

        #region Event Functions

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _animator = GetComponent<Animator>();

            if (_toggle is CustomToggle customToggle)
            {
                _hubNavigationToggle = customToggle;
            }
        }

        private void OnEnable()
        {
            _toggle.onValueChanged.AddListener(PlayAnimation);

            if (_hubNavigationToggle != null)
            {
                _hubNavigationToggle.OnInstantValueChanged += PlayInstantAnimation;
            }

            //PlayInstantAnimation(true);
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(PlayAnimation);

            if (_hubNavigationToggle != null)
            {
                _hubNavigationToggle.OnInstantValueChanged -= PlayInstantAnimation;
            }
        }

        #endregion

        private void PlayAnimation(bool isOn)
        {
            _animator.SetBool(_isOnHash, isOn);
        }

        private void PlayInstantAnimation(bool isOn)
        {
            var animationHash = isOn ? _isToggleOnAnimationHash : _isToggleOffAnimationHash;
            _animator.SetBool(_isOnHash, isOn);
            _animator.Play(animationHash, 0, 1f);
        }
    }
}