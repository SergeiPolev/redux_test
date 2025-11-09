using Services;
using System.Collections.Generic;
using UnityEngine;

namespace DebugGame.UI
{

    public enum DebugPage
    {
        Home,
        General,
        Level,
    }

    public class UIDebug : WindowBase
    {
        public override WindowId WindowID => WindowId.DEBUG;

        [SerializeField] private List<UIDebugPage> _pages;
        
        private UIDebugPage _currentPage;
        public DebugService GameService { get; private set; }

        public UIDebugPage DebugPage => _currentPage;
        
        protected override void _Initialize(AllServices services)
        {
            GameService = services.Single<DebugService>();
        }

        public void ReInit()
        {
            for (int i = 0; i < _pages.Count; i++)
            {
                _pages[i].Init(this);
            }
        }

        protected override void _Open()
        {
            gameObject.SetActive(true);
            CloseAllPage();
            ReInit();

            AllServices.Container.Single<WindowService>().Open(WindowId.DEBUG_OPEN);
        }

        protected override void _Close()
        {
            base._Close();
            
            AllServices.Container.Single<WindowService>().Close(WindowId.DEBUG_OPEN);
        }

        public void OpenPage(DebugPage id)
        {
            var page = _pages.Find(p => p.ID == id);
            if (page != null)
            {
                if (_currentPage != null)
                    _currentPage.Close();
                _currentPage = page;
                _currentPage.Open();
            }
            else
            {
                Debug.LogError($"UIDebug. Fail open page {id}");
            }
        }


        public void CloseAllPage()
        {
            for (int i = 0; i < _pages.Count; i++)
            {
                if (_pages[i].IsOpen)
                    _pages[i].Close();
            }
        }
    }

}