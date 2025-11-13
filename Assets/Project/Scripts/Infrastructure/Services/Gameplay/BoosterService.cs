using System;
using System.Collections.Generic;
using Boosters;
using Infrastructure.StateMachine;
using UnityEngine;

namespace Infrastructure.Services.Gameplay
{
    public enum BoosterId
    {
        PaintUpperHexes,
        ShufflePiles,
        BreakPiles,
    }

    public class BoosterService : IService, ITick, IDisposable
    {
        private readonly List<IBooster> _activeBoosters = new();
        private AllServices _services;

        public void Dispose()
        {
            for (var i = _activeBoosters.Count - 1; i >= 0; i--)
            {
                var booster = _activeBoosters[i];
                booster.Deactivate();
                _activeBoosters.RemoveAt(i);
            }
        }

        public void Tick()
        {
            for (var i = _activeBoosters.Count - 1; i >= 0; i--)
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
    }
}