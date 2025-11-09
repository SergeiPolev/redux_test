using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelItemUI : MonoBehaviour
{
    public Sprite[] Visuals;
    public Image Icon;
    public Image Outline;
    public CanvasGroup NewItemGroup;
    public Image NewItemIcon;
    public TextMeshProUGUI LevelText;

    public void SetLevelNumber(int levelNumber, bool isCurrent)
    {
        LevelText.SetText($"LEVEL {levelNumber}");
        Icon.sprite = Visuals[(levelNumber - 1) % Visuals.Length];
        NewItemGroup.alpha = 0;
        Outline.gameObject.SetActive(isCurrent);
    }
}