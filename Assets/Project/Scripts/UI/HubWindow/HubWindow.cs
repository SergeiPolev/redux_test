using System;
using Infrastructure.Services;
using Infrastructure.Services.Progress;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HubWindow
{
    public class HubWindow : WindowBase
    {
        #region Serialized Fields

        [SerializeField] private Button _startButton;

        [Header("Level Selection")] [SerializeField]
        private TMP_Text _lvl;

        [Header("Level Items")] [SerializeField]
        private LevelItemUI[] _levelItems;

        [SerializeField] private RectTransform _levelItemRoot;

        #endregion

        private LevelProgressService _lvlProgress;
        public override WindowId WindowID => WindowId.HUB;

        public event Action OnGameStartEvent;

        protected override void _Initialize(AllServices services)
        {
            _lvlProgress = AllServices.Container.Single<LevelProgressService>();
        }

        protected override void _Open()
        {
            base._Open();

            UpdateLevelText();
            //UpdateLevelIcons();

            _startButton.onClick.AddListener(ClickStartButton);
        }

        private void UpdateLevelIcons()
        {
            var currentLevel = _lvlProgress.CurrentLevelNumber;

            for (var i = 0; i < _levelItems.Length; i++)
            {
                _levelItems[i].SetLevelNumber(currentLevel + i, i == 0);
            }
        }

        protected override void _Close()
        {
            base._Close();
            _startButton.onClick.RemoveListener(ClickStartButton);
        }

        private void ClickStartButton()
        {
            OnGameStartEvent?.Invoke();
        }

        public void OnNextLvl()
        {
            _lvlProgress.SetNextLevel(true);
            UpdateLevelText();
        }

        public void OnPrevLvl()
        {
            _lvlProgress.SetPreviousLevel();
            UpdateLevelText();
        }

        private void UpdateLevelText()
        {
            _lvl.SetText($"Level {_lvlProgress.CurrentLevelNumber}");
        }
    }
}