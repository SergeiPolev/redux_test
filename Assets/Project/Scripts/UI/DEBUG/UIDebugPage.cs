using UnityEngine;

namespace DebugGame.UI
{
    public abstract class UIDebugPage : MonoBehaviour
    {
        public abstract DebugPage ID { get; }
        [SerializeField] private GameObject _page;
        protected UIDebug UIDebug { get; private set; }

        protected DebugService DebugService => UIDebug.GameService;
        public bool IsOpen => _page.activeSelf;

        public void Init(UIDebug debug)
        {
            UIDebug = debug;
            _Init();
        }
        protected virtual void _Init() { }

        public virtual void Open()
        {
            _page.SetActive(true);
        }

        public virtual void Close()
        {
            _page.SetActive(false);
        }

        public void OnGoHome()
        {
            UIDebug.OpenPage(DebugPage.Home);
        }

        public void OnCloseDebug()
        {
            UIDebug.CloseAllPage();
        }
    }
}