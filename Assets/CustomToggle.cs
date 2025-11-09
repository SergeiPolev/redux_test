using System;
using UnityEngine.UI;

public class CustomToggle : Toggle
{
    public event Action<bool> OnInstantValueChanged;
    public void SetValue(bool isOnValue)
    {
        isOn = isOnValue;
        OnInstantValueChanged?.Invoke(isOn);
    }
}