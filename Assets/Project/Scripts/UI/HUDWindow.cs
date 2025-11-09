using Services;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDWindow : WindowBase
{
    [System.Serializable]
    private class CurrencyPlace
    {
        public GameCurrencyID ID;
        public RectTransform Target;
    }

    public override WindowId WindowID => WindowId.HUD;
    private LevelProgressService _levelProgressService;
    private GoogleSheetService _googleSheetService;
    private GameWalletService _wallet;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private List<CurrencyPlace> _collectableTarget;

    public Vector3 CollectableTaget(GameCurrencyID id) =>
        _collectableTarget.Find(i => i.ID == id).Target.position;

    protected override void _Initialize(AllServices services)
    {
        _levelProgressService = services.Single<LevelProgressService>();
        _googleSheetService = services.Single<GoogleSheetService>();
        _wallet = services.Single<GameWalletService>();
    }


    protected override void _Open()
    {
        base._Open();

        var lvl = _levelProgressService.CurrentLevelNumber;
        var stage = _levelProgressService.CurrentStageNumber;
        _level.SetText($"Level {lvl}");
    }
}
