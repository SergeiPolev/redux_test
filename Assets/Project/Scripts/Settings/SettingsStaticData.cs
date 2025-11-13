using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "SettingsStaticData", menuName = "ScriptableObjects/StaticData/Settings")]
    public class SettingsStaticData : ScriptableObject
    {
        #region Serialized Fields

        public bool UseJSONAsConfig;
        public string MasterURL;

        #endregion
    }
}