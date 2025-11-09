using System.Collections.Generic;
using UnityEngine;
using Services;
using UnityEngine.UI;

public class HubNavigationWindow : WindowBase
{
    [SerializeField] private List<HubNavigationToggle> _toggles;
    [SerializeField] private ToggleGroup _toggleGroup;
    
    private WindowId _currentWindow;
    private WindowId _defaultWindow = WindowId.HUB;
    
    private WindowService _windowService => AllServices.Container.Single<WindowService>();

    protected override void _Open()
    {
        foreach (var toggle in _toggles)
        {
            if (toggle.ToggleWindow == WindowId.None)
            {
                continue;
            }
            
            toggle.CustomToggle.group = _toggleGroup;
            toggle.CustomToggle.onValueChanged.AddListener(ClickToggle);
        }
        
        base._Open();
        
        SetupToggles();
    }

    private void SetupToggles()
    {
        for (int i = 0; i < _toggles.Count; i++)
        {
            _toggles[i].CustomToggle.SetValue(_toggles[i].ToggleWindow == _defaultWindow);
        }

        SwitchWindow(_defaultWindow);
    }

    protected override void _Close()
    {
        CloseCurrentWindow();
        _currentWindow = WindowId.None;
        
        foreach (var toggle in _toggles)
        {
            toggle.CustomToggle.onValueChanged.RemoveListener(ClickToggle);
        }
        
        base._Close();
    }

    private void SwitchWindow(WindowId windowId)
    {
        if (windowId == _currentWindow)
        {
            return;
        }

        CloseCurrentWindow();
        _currentWindow = windowId;
        _windowService.Open(_currentWindow);
    }

    private void CloseCurrentWindow()
    {
        if (_currentWindow != WindowId.None)
        {
            _windowService.Close(_currentWindow);
        }
    }

    private void ClickToggle(bool isOn)
    {
        if (isOn)
        {
            for (int i = 0; i < _toggles.Count; i++)
            {
                if (_toggles[i].CustomToggle.isOn)
                {
                    SwitchWindow(_toggles[i].ToggleWindow);
                }
            }
            
            PlayClick();
        }
    }

    public override WindowId WindowID => WindowId.HUB_Navigation;
}