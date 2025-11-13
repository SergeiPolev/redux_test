using System;
using DG.Tweening;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FadeWindow
{
    public class FadeWindowModel : WindowModel
    {
        public Color Color;
        public float FadeInDelay;
        public float FadeInDuration;
        public float FadeOutDelay;
        public float FadeOutDuration;
        public Action OnFadeInAction;

        public FadeWindowModel(float fadeInDelay, float fadeInDuration, float fadeOutDelay, float fadeOutDuration,
            Action onFadeInAction)
        {
            FadeInDelay = fadeInDelay;
            FadeInDuration = fadeInDuration;
            FadeOutDelay = fadeOutDelay;
            FadeOutDuration = fadeOutDuration;
            OnFadeInAction = onFadeInAction;
            Color = Color.black;
            Color.a = 0;
        }

        public FadeWindowModel(float fadeInDelay, float fadeInDuration, float fadeOutDelay, float fadeOutDuration,
            Action onFadeInAction, Color color)
        {
            FadeInDelay = fadeInDelay;
            FadeInDuration = fadeInDuration;
            FadeOutDelay = fadeOutDelay;
            FadeOutDuration = fadeOutDuration;
            OnFadeInAction = onFadeInAction;
            Color = color;
            Color.a = 0;
        }

        public FadeWindowModel(float fadeInDelay, float fadeInDuration, float fadeOutDelay, float fadeOutDuration,
            Action onFadeInAction, Color color,
            Action onCloseAction) : base(onCloseAction)
        {
            FadeInDelay = fadeInDelay;
            FadeInDuration = fadeInDuration;
            FadeOutDelay = fadeOutDelay;
            FadeOutDuration = fadeOutDuration;
            OnFadeInAction = onFadeInAction;
            Color = color;
            Color.a = 0;
        }
    }

    public class TransitionFadeWindow : WindowBase
    {
        #region Serialized Fields

        [SerializeField] private Image _background;

        #endregion

        private Sequence _fadeSequence;
        private FadeWindowModel _fadeWindowModel;
        public override WindowId WindowID => WindowId.TransitionFade;

        protected override void _Open()
        {
            base._Open();
            RefreshWindow();
        }

        protected void RefreshWindow()
        {
            var color = _background.color;
            color.a = 0;
            _background.color = color;
        }

        public void SetModel(WindowModel windowModel)
        {
            //base.SetModel(windowModel);

            if (windowModel is FadeWindowModel fadeModel)
            {
                _fadeWindowModel = fadeModel;
                _background.color = fadeModel.Color;
                _fadeSequence = DOTween.Sequence();
                _fadeSequence.Append(_background.DOFade(1f, fadeModel.FadeInDuration).SetDelay(fadeModel.FadeInDelay)
                    .OnComplete(OnFadeIn));
                _fadeSequence.Append(_background.DOFade(0f, fadeModel.FadeOutDuration)
                    .SetDelay(fadeModel.FadeOutDelay));
                _fadeSequence.SetUpdate(true);
                _fadeSequence.OnComplete(Close);
            }
            else
            {
                throw new Exception("Dont find FadeWindowModel");
            }
        }

        private void OnFadeIn()
        {
            _fadeWindowModel?.OnFadeInAction?.Invoke();
        }
    }
}