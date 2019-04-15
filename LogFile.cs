using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WriteLogDigiRite
{
    public class LogFile : IDisposable
    {
        public LogFile(String LogPath, bool append = true)
        {
            startlog(LogPath, append);
        }

        private void startlog(String LogPath, bool append)
        {
            logFile = new StreamWriter(LogPath, true);
            logFile.WriteLine(String.Format("{0:yyyy/MM/dd HH:mm:ss}", DateTime.UtcNow));
        }

        public static long LogFileLength(String LogPath)
        {
           var fi = new FileInfo(LogPath);
            fi.Refresh();
            return fi.Length;
        }

        public void SendToLog(String s)
        {
            logFile.Write(String.Format("{0:HH:mm:ss} ", DateTime.UtcNow));
            logFile.WriteLine(s);
        }

        public void Flush()
        { logFile.Flush();    }

        public void ResetToEmpty(String LogPath)
        {
            if (logFile != null)
                logFile.Dispose();
            System.IO.File.Delete(LogPath);
            startlog(LogPath, false);
        }

        public void Dispose()
        {
            ((IDisposable)logFile).Dispose();
        }

        private System.IO.StreamWriter logFile;
    }
}
