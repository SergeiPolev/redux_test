using UnityEngine;

namespace Tools.Logger
{
    public class LoggerDebugStarter : MonoBehaviour
    {
#if DEBUG
        private void Start()
        {
            Logger.Initialization();
        }

        private void OnDestroy()
        {
            Logger.Dispose();
        }

        [ContextMenu("Open Folder")]
        private void OpenFolder()
        {
            Logger.OpenFolder();
        }
#endif
    }
}