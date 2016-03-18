using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer_V2._0._03152016
{
    class Log
    {
#if DEBUG
        private static string LogDirectory = @"C:\ImageResizer";
#else
        private static string LogDirectory = @"D:\ImageResizer";
#endif
        public static string LogPath = Path.Combine(LogDirectory, "ImageResizeLog.txt");

        public static void WriteToLog(string message)
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);

            File.AppendAllText(LogPath, "[" + DateTime.Now + "] " + message + "\r\n");
        }

        public static void WriteToEventLog(string message, EventLogEntryType typeOfEvent)
        {
            if (!EventLog.SourceExists("Image Resizer"))
                EventLog.CreateEventSource("Image Resizer", "Application");
            EventLog.WriteEntry("Image Resizer", message, typeOfEvent);
        }

        public static void ClearLog()
        {
            WriteToLog("");
            File.WriteAllText(LogPath, string.Empty);
        }

        public static void CloseWithSuccess()
        {
            Console.WriteLine("Completed");
            File.AppendAllText(LogPath, "[" + DateTime.Now + "] Process completed successfully.");
        }

        public static void CloseWithErrors()
        {
            Console.WriteLine("Failed");
            File.AppendAllText(LogPath, "[" + DateTime.Now + "] Process completed with errors.");
            WriteToEventLog("ImageResizer Failed. Log saved at " + LogPath, EventLogEntryType.Error);
        }
    }
}
