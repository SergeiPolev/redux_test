using System.IO;
using UnityEngine;
using LoggerFile.Element;

namespace LoggerFile
{
    public static class Logger 
    {
        private static string _workDirectory = $"{Application.persistentDataPath}/Logs";
        private static LoggerFileWriter _writer;
        private static bool isInit;

        public static void Initialization()
        {
            if (isInit)
            {
                Debug.LogWarning("Logger. Initialization already done!");
                return;
            }

            if (Directory.Exists(_workDirectory) == false)
                Directory.CreateDirectory(_workDirectory);

            _writer = new LoggerFileWriter(_workDirectory);
            Application.logMessageReceivedThreaded += LogMessageReceived;
            isInit = true;
        }

        private static void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            switch(type)
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
            UnityEditor.EditorUtility.RevealInFinder(_workDirectory);
#endif
        }
    }
}
