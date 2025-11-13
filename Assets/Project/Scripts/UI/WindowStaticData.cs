using System.Collections.Generic;
using UI.Base;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "WindowStaticData", menuName = "ScriptableObjects/StaticData/Window")]
    public class WindowStaticData : ScriptableObject
    {
        #region Serialized Fields

        public List<WindowBase> Configs;

        #endregion
    }
}