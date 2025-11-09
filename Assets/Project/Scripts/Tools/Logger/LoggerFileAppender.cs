using System.IO;
using System.Text;

namespace LoggerFile.Element
{
    public class LoggerFileAppender
    {
        private readonly object _lockObject = new object();
        public string FileName { get; }

        public LoggerFileAppender(string file)
        {
            FileName = file;
        }

        public bool Append(string content)
        {
            try
            {
                lock(_lockObject)
                {
                    using (FileStream fs = File.Open(FileName, FileMode.Append, FileAccess.Write, FileShare.Read))
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
