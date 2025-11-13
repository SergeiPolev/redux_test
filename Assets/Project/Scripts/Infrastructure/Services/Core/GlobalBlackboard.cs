using System.Collections.Generic;
using Infrastructure.Services.Gameplay;
using UnityEngine;

namespace Infrastructure.Services.Core
{
    public class GlobalBlackboard : IService
    {
        public Dictionary<ColorID, Color> ColorsByID = new();
        public Dictionary<ColorID, Material> MaterialsByColorID = new();

        public void Initialize()
        {
        }
    }
}