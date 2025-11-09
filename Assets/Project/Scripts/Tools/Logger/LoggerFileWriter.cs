using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace LoggerFile.Element
{
    public class LoggerFileWriter
    {
        private readonly string _folder;
        private string _filePath;
        private LoggerFileAppender _appender;
        private Thread _workThread;
        private bool _disposing;
        private readonly ConcurrentQueue<LogMessage> _messages = new ConcurrentQueue<LogMessage>();
        private readonly ManualResetEvent _mre = new ManualResetEvent(true);

        private const string DateFormat = "yyyy-MM-dd HH-mm-ss";
        private const string MessageFormat = "{0:dd/MM/yyyy HH:mm:ss:ffff} [{1}]: {2}\r";
        private const int MAX_MESSAGE_LENGHT = 3500;

        public LoggerFileWriter(string folder)
        {
            _folder = folder;
            _filePath = $"{_folder}/{Application.productName} ({DateTime.UtcNow.ToString(DateFormat)}).log";
            _workThread = new Thread(StoreMessage)
            {
                IsBackground = true,
                Priority = System.Threading.ThreadPriority.BelowNormal
            };
            _workThread.Start();
        }

        public void Write(LogMessage message)
        {
            try
            {
                if(message.Message.Length>MAX_MESSAGE_LENGHT)
                {
                    var preview = $"Message is to long {message.Message.Length}. Preview: {message.Message.Substring(0, MAX_MESSAGE_LENGHT)}";
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
            while (_disposing == false)
            {
                while (_messages.IsEmpty == false)
                {
                    try
                    {
                        LogMessage message;
                        if (_messages.TryPeek(out message) == false)
                        {
                            Thread.Sleep(5);
                            continue;
                        }

                        if (_appender == null || _appender.FileName != _filePath)
                            _appender = new LoggerFileAppender(_filePath);


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
