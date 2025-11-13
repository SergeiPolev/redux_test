using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using ThreadPriority = System.Threading.ThreadPriority;

namespace Tools.Logger
{
    public class LoggerFileWriter
    {
        private const string DateFormat = "yyyy-MM-dd HH-mm-ss";
        private const string MessageFormat = "{0:dd/MM/yyyy HH:mm:ss:ffff} [{1}]: {2}\r";
        private const int MAX_MESSAGE_LENGHT = 3500;
        private readonly string _filePath;
        private readonly string _folder;
        private readonly ConcurrentQueue<LogMessage> _messages = new();
        private readonly ManualResetEvent _mre = new(true);
        private readonly Thread _workThread;
        private LoggerFileAppender _appender;
        private bool _disposing;

        public LoggerFileWriter(string folder)
        {
            _folder = folder;
            _filePath = $"{_folder}/{Application.productName} ({DateTime.UtcNow.ToString(DateFormat)}).log";
            _workThread = new Thread(StoreMessage)
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
            };
            _workThread.Start();
        }

        public void Write(LogMessage message)
        {
            try
            {
                if (message.Message.Length > MAX_MESSAGE_LENGHT)
                {
                    var preview =
                        $"Message is to long {message.Message.Length}. Preview: {message.Message.Substring(0, MAX_MESSAGE_LENGHT)}";
                    _messages.Enqueue(new LogMessage(preview, message.Type) { Time = message.Time });
                }
                else
                {
                    _messages.Enqueue(message);
                }

                _mre.Set();
            }
            catch
            {
            }
        }

        private void StoreMessage()
        {
            while (!_disposing)
            {
                while (!_messages.IsEmpty)
                {
                    try
                    {
                        LogMessage message;
                        if (!_messages.TryPeek(out message))
                        {
                            Thread.Sleep(5);
                            continue;
                        }

                        if (_appender == null || _appender.FileName != _filePath)
                        {
                            _appender = new LoggerFileAppender(_filePath);
                        }


                        var writeMassage = string.Format(MessageFormat, message.Time, message.Type, message.Message);
                        if (_appender.Append(writeMassage))
                        {
                            _messages.TryDequeue(out message);
                        }
                        else
                        {
                            Thread.Sleep(5);
                        }
                    }
                    catch
                    {
                        break;
                    }
                }

                _mre.Reset();
                _mre.WaitOne(500);
            }
        }

        public void Dispose()
        {
            _disposing = true;
            _workThread?.Abort();
            GC.SuppressFinalize(this);
        }
    }
}