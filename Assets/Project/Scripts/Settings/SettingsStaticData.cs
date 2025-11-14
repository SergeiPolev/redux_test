using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "SettingsStaticData", menuName = "ScriptableObjects/StaticData/Settings")]
    public class SettingsStaticData : ScriptableObject
    {
#region Default Values
        [Header("Default Values")]
        public float RaycastDistance = 100f;
        public Vector3 DragOffset = new(0, 0.5f, 0.5f);
        public Vector3 TapToChooseOffset = new(0, 0, 0.25f);
#endregion

#region Animations
        [Header("Animation Settings")]
        public float HammerJumpHeight = 1;
        public float HammerJumpDuration = 0.1f;
        
        public float HoverHeight = 0.4f;
        public float HoverDuration = 0.175f;

        public float PilePlaceHeight = 0.5f;
        public float PilePlaceDuration = 0.15f;
        
        public float FillCellHeight = -0.1f;
        public float FillCellDuration = 0.175f;
        
        public float HammeredHeight = -0.4f;
        public float HammeredDuration = 0.25f;

#endregion
    }
}