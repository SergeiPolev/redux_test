using System;
using UnityEngine;

namespace LoggerFile.Element
{
    public class LogMessage
    {
        public string Message { get; set; }
        public LogType Type  { get; set; }
        public DateTime Time { get; set; }

        public LogMessage(string message, LogType type)
        {
            Message = message;
            Type = type;
            Time = DateTime.UtcNow;
        }
    }
}
