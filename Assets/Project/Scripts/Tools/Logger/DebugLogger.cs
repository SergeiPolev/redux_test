using UnityEngine;

namespace Tools.Logger
{
    public static class DebugLogger
    {
        public static void Log(string msg)
        {
#if DEBUG
            Debug.Log(msg);
#endif
        }

        public static void LogFormat(string msg, params object[] args)
        {
#if DEBUG
            Debug.LogFormat(msg, args);
#endif
        }

        public static void LogWarning(string msg)
        {
#if DEBUG
            Debug.LogWarning(msg);
#endif
        }

        public static void LogWarningFormat(string msg, params object[] args)
        {
#if DEBUG
            Debug.LogWarningFormat(msg, args);
#endif
        }

        public static void LogError(string msg)
        {
#if DEBUG
            Debug.LogError(msg);
#endif
        }

        public static void LogErrorFormat(string msg, params object[] args)
        {
#if DEBUG
            Debug.LogErrorFormat(msg, args);
#endif
        }
    }
}