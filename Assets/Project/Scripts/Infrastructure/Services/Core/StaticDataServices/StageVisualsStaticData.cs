using System;
using UnityEngine;

namespace Infrastructure.Services.Core
{
    [CreateAssetMenu(fileName = "StageVisualsStaticData", menuName = "ScriptableObjects/StaticData/Stage Visuals")]
    public class StageVisualsStaticData : ScriptableObject
    {
        #region Serialized Fields

        public StageVisual[] Visuals;

        public Vector4 ShadowOffset;

        #endregion

        /*public Vector4 OutlineOffset;
    public float OutlineScale = 1.5f;*/
    }

    [Serializable]
    public struct StageVisual
    {
        public Sprite SkyVisual;
        public Texture BackgroundVisual;
        public Texture BlocksVisual;
        public Color CameraColor;
    }
}