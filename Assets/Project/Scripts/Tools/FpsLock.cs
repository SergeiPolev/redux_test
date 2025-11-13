using UnityEngine;

namespace Tools
{
    public class FpsLock : MonoBehaviour
    {
        #region Event Functions

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            Application.targetFrameRate = 60;
        }

        #endregion
    }
}