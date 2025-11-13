using System;
using Infrastructure.Services;
using UnityEngine;

namespace UI.Base
{
    public abstract class WindowBase : MonoBehaviour
    {
        public abstract WindowId WindowID { get; }

        public virtual bool IsOpen => gameObject.activeSelf;
        public event Action<WindowBase> OnOpenE, OnCloseE;

        public void Initialize(AllServices services)
        {
            _Initialize(services);
        }

        protected virtual void _Initialize(AllServices services)
        {
        }

        public void Open()
        {
            if (IsOpen)
            {
                return;
            }

            _Open();
            OnOpenE?.Invoke(this);
        }

        protected virtual void _Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            if (!IsOpen)
            {
                return;
            }

            _Close();
            OnCloseE?.Invoke(this);
        }

        protected virtual void _Close()
        {
            gameObject.SetActive(false);
        }

        protected virtual void PlayClick()
        {
        }
    }
}