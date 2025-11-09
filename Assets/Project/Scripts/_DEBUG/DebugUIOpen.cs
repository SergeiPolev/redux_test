using DebugGame.UI;
using Services;
using UnityEngine;

public class DebugUIOpen : WindowBase
{
    [SerializeField] private int _countTarget = 3;
    [SerializeField] private float _minInterval = 0.5f;
    
    private UIDebug _uiDebug;

    private int _count;
    private float _lastTimeClick;
    
    protected override void _Open()
    {
        base._Open();

        _uiDebug = AllServices.Container.Single<WindowService>().GetOrCreateWindow<UIDebug>(WindowId.DEBUG);
    }

    public void OnClick()
    {
        if (Time.unscaledTime - _lastTimeClick <= _minInterval)
        {
            _count++;
            if (_count >= _countTarget)
            {
                if (_uiDebug.DebugPage == null)
                    _uiDebug.OpenPage(DebugPage.Home);
                else
                    _uiDebug.OpenPage(_uiDebug.DebugPage.ID);
                _count = 1;
            }
        }
        else
        {
            _count = 1;
        }
        _lastTimeClick = Time.unscaledTime;
    }

    public override WindowId WindowID => WindowId.DEBUG_OPEN;
}