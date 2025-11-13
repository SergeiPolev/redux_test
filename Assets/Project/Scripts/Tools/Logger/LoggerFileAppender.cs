using System.IO;
using System.Text;

namespace Tools.Logger
{
    public class LoggerFileAppender
    {
        private readonly object _lockObject = new();

        public LoggerFileAppender(string file)
        {
            FileName = file;
        }

        public string FileName { get; }

        public bool Append(string content)
        {
            try
            {
                lock (_lockObject)
                {
                    using (var fs = File.Open(FileName, FileMode.Append, FileAccess.Write, FileShare.Read))
                    {
                        var bytes = Encoding.UTF8.GetBytes(content);
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}