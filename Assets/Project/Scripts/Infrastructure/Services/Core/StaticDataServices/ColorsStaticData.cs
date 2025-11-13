using System;
using Infrastructure.Services.Gameplay;
using UnityEngine;

namespace Infrastructure.Services.Core
{
    [CreateAssetMenu(fileName = "Colors Static Data", menuName = "Game/Colors/Data")]
    public class ColorsStaticData : ScriptableObject
    {
        #region Serialized Fields

        public ColorTuple[] Colors;

        #endregion
    }

    [Serializable]
    public struct ColorTuple
    {
        public ColorID ColorID;
        public Color Color;
        public Material Material;
    }
}