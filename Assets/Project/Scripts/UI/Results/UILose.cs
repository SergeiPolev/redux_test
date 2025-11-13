using System;
using DG.Tweening;
using UI.Base;
using UnityEngine;

namespace UI.Results
{
    public class UILose : WindowBase
    {
        #region Serialized Fields

        [SerializeField] private CanvasGroup _group;

        #endregion

        public Action OnHubE;
        public override WindowId WindowID => WindowId.Lose;

        protected override void _Open()
        {
            base._Open();
            _group.alpha = 0;
            _group.DOFade(1, 0.5f);
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

        public void SetOnHubEvent(Action action)
        {
            OnHubE = action;
        }
    }
}