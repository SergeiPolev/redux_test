using UnityEngine;
using System;
using System.Collections.Generic;

public enum BoosterId
{
    PaintUpperHexes,
    ShufflePiles,
    BreakPiles,
}

namespace Services
{
    public class BoosterService : IService, ITick, IDisposable
    {
        private AllServices _services;
        
        private readonly List<IBooster> _activeBoosters = new();

        public void Initialize(AllServices services)
        {
            _services = services;
        }

        public void ActivateBooster(BoosterId boosterId)
        {
            if (_activeBoosters.Count > 0 || _activeBoosters.Find(x => x.BoosterId == boosterId) != null)
            {
                Debug.Log($"Booster {boosterId} is already active");
                return;
            }

            var booster = BuildBooster(boosterId);
            _activeBoosters.Add(booster);
            booster.Activate();
        }

        public void Tick()
        {
            for (int i = _activeBoosters.Count - 1; i >= 0; i--)
            {
                var booster = _activeBoosters[i];
                booster.Tick();

                if (!booster.IsActive)
                {
                    booster.Deactivate();
                    _activeBoosters.RemoveAt(i);
                }
            }
        }

        private IBooster BuildBooster(BoosterId boosterId)
        {
            switch (boosterId)
            {
                case BoosterId.PaintUpperHexes:
                    return new PaintUpperHexesBooster(_services);

                case BoosterId.ShufflePiles:
                    return new ShufflePilesBooster(_services);
                
                case BoosterId.BreakPiles:
                    return new BreakPilesBooster(_services);
                
                default:
                    throw new NotSupportedException($"Booster {boosterId} not supported");
            }
        }

        public bool HasBooster<T>() where T : IBooster
        {
            if (_activeBoosters.Count == 0)
            {
                return false;
            }

            foreach (var booster in _activeBoosters)
            {
                if (booster is T)
                {
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            for (int i = _activeBoosters.Count - 1; i >= 0; i--)
            {
                var booster = _activeBoosters[i];
                booster.Deactivate();
                _activeBoosters.RemoveAt(i);
            }
        }
    }
}

