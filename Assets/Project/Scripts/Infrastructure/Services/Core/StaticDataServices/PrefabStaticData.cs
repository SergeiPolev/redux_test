using Boosters.Views;
using Hexes;
using UnityEngine;

namespace Infrastructure.Services.Core
{
    [CreateAssetMenu(fileName = "PrefabStaticData", menuName = "ScriptableObjects/StaticData/Prefabs")]
    public class PrefabStaticData : ScriptableObject
    {
        #region Serialized Fields

        public HexModelView HexModelView;
        public HexCellView HexCellView;
        public HexPile HexPile;
        public BrushView BrushView;
        public HammerView HammerView;

        #endregion
    }
}