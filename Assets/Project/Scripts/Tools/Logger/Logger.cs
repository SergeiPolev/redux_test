using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tools.Logger
{
    public static class Logger
    {
        private static readonly string _workDirectory = $"{Application.persistentDataPath}/Logs";
        private static LoggerFileWriter _writer;
        private static bool isInit;

        public static void Initialization()
        {
            if (isInit)
            {
                Debug.LogWarning("Logger. Initialization already done!");
                return;
            }

            if (!Directory.Exists(_workDirectory))
            {
                Directory.CreateDirectory(_workDirectory);
            }

            _writer = new LoggerFileWriter(_workDirectory);
            Application.logMessageReceivedThreaded += LogMessageReceived;
            isInit = true;
        }

        private static void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Exception:
                    _writer.Write(new LogMessage(condition, type));
                    _writer.Write(new LogMessage(stackTrace, type));
                    break;
                default:
                    _writer.Write(new LogMessage(condition, type));
                    break;
            }
        }

        public static void Dispose()
        {
            Application.logMessageReceivedThreaded -= LogMessageReceived;
            _writer.Dispose();
            isInit = false;
        }

        public static void OpenFolder()
        {
#if UNITY_EDITOR
            EditorUtility.RevealInFinder(_workDirectory);
#endif
        }
    }
}