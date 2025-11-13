using System;
using UnityEngine;

namespace Tools.Logger
{
    public class LogMessage
    {
        public LogMessage(string message, LogType type)
        {
            Message = message;
            Type = type;
            Time = DateTime.UtcNow;
        }

        public string Message { get; set; }
        public LogType Type { get; set; }
        public DateTime Time { get; set; }
    }
}