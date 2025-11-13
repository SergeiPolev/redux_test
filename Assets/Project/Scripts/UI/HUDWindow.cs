using Infrastructure.Services;
using Infrastructure.Services.Gameplay;
using Infrastructure.Services.Progress;
using TMPro;
using UI.Base;
using UI.Boosters;
using UnityEngine;

namespace UI
{
    public class HUDWindow : WindowBase
    {
        #region Serialized Fields

        [SerializeField] private TMP_Text _level;
        [SerializeField] private ProgressBar _progressBar;
        [SerializeField] private BoosterButton[] _boosterButtons;

        #endregion

        private LevelProgressService _levelProgressService;
        public override WindowId WindowID => WindowId.HUD;

        protected override void _Initialize(AllServices services)
        {
            _levelProgressService = services.Single<LevelProgressService>();
            services.Single<ClearStackService>().OnClearCompleted += UpdateProgressBar;

            foreach (var boosterButton in _boosterButtons)
            {
                boosterButton.Initialize(services.Single<BoosterService>());
            }
        }

        private void UpdateProgressBar()
        {
            _progressBar.UpdateValue(_levelProgressService.HexesCurrent, _levelProgressService.HexesGoal);
        }

        protected override void _Open()
        {
            base._Open();

            var lvl = _levelProgressService.CurrentLevelNumber;
            _level.SetText($"Level {lvl}");
            _progressBar.Initialize();
            UpdateProgressBar();
        }
    }
}