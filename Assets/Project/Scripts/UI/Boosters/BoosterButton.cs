using Infrastructure.Services.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Boosters
{
    public class BoosterButton : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private BoosterId _boosterId;

        #endregion

        private BoosterService _boosterService;

        private Button _button;

        public BoosterId BoosterId => _boosterId;

        public void Initialize(BoosterService boosterService)
        {
            _boosterService = boosterService;
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Click);
        }

        private void Click()
        {
            _boosterService.ActivateBooster(_boosterId);
        }
    }
}