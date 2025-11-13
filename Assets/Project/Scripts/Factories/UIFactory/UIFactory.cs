using Infrastructure.Services;
using Infrastructure.Services.Core;
using UI;
using UI.Base;
using UnityEngine;

namespace Factories.UIFactory
{
    public class UIFactory : IService
    {
        private const string UIRootPath = "UI/UIRoot";
        private AllServices _services;
        private StaticDataService _staticData;

        public UIFactory()
        {
            CreateUIRoot();
        }

        public Transform UIRoot { get; private set; }

        public void Initialize(AllServices services)
        {
            _services = services;
            _staticData = _services.Single<StaticDataService>();
        }

        public T CreateWindow<T>(WindowId windowId) where T : WindowBase
        {
            var winPrefab = _staticData.ForWindow(windowId);
            var window = Object.Instantiate(winPrefab, UIRoot) as T;
            window.Initialize(_services);
            window.gameObject.SetActive(false);
            return window;
        }

        private void CreateUIRoot()
        {
            UIRoot = Object.Instantiate(Resources.Load<GameObject>(UIRootPath)).transform;

            if (UIRoot.TryGetComponent(out Canvas canvas))
            {
                canvas.worldCamera = Camera.main;
            }
        }
    }
}