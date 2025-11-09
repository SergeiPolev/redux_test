using LoggerFile;
using UnityEngine;
using Logger = LoggerFile.Logger;

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

    [UnityEngine.ContextMenu("Open Folder")]
    private void OpenFolder()
    {
        Logger.OpenFolder();
    }
#endif
}
