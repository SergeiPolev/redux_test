using System;
using DG.Tweening;
using UI.Base;
using UnityEngine;

namespace UI.Results
{
    public class UIWin : WindowBase
    {
        #region Serialized Fields

        [SerializeField] private CanvasGroup _group;
        [SerializeField] private CanvasGroup _rewardGroup;

        #endregion

        public override WindowId WindowID => WindowId.Win;
        public event Action OnHubE;

        protected override void _Open()
        {
            base._Open();
            _group.alpha = 0;
            _group.DOFade(1, 0.5f);
        }

        public void SetOnHubEvent(Action action)
        {
            OnHubE = action;
        }

        protected override void _Close()
        {
            DOTween.Sequence(this)
                .Append(_group.DOFade(0, 0.5f))
                .AppendCallback(() => { base._Close(); });
        }

        public void OnHub()
        {
            OnHubE?.Invoke();
            Close();
        }
    }
}