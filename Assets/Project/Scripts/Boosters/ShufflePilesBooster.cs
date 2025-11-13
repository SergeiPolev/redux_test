using System;
using Infrastructure.Services;
using Infrastructure.Services.Gameplay;
using UnityEngine;

namespace Boosters
{
    public class ShufflePilesBooster : IBooster
    {
        private readonly HexPilesService _pilesService;
        private float _timer;

        public ShufflePilesBooster(AllServices services)
        {
            _pilesService = services.Single<HexPilesService>();
        }

        public event Action<BoosterProgressEvent> OnProgressChanged;

        public event Action OnActivated;
        public event Action OnDeactivated;

        public bool IsActive { get; private set; }

        public BoosterId BoosterId => BoosterId.ShufflePiles;

        public void Activate()
        {
            IsActive = true;

            _timer = 0;
            _pilesService.ShufflePiles();
            OnActivated?.Invoke();
        }

        public void Tick()
        {
            if (!IsActive)
            {
                return;
            }

            _timer += Time.deltaTime;

            if (_timer >= 1f)
            {
                IsActive = false;
            }

            OnProgressChanged?.Invoke(new BoosterProgressEvent(_timer, 1, 1 - _timer));
        }

        public void Deactivate()
        {
            IsActive = false;

            OnDeactivated?.Invoke();
        }
    }
}