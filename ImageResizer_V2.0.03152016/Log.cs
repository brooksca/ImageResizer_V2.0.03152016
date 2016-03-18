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
        public static List<string> EventLogMessages;
        public static DateTime StartTime;

        public static void WriteToLog(string message)
        {
            File.AppendAllText(LogPath, "[" + DateTime.Now + "] " + message + "\r\n");
        }

        public static void WriteToEventLog(string message)
        {
            EventLogMessages.Add(message);
        }

        public static void BeginLog()
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
            File.WriteAllText(LogPath, "");
            WriteToLog("Program start");
            EventLogMessages = new List<string>();
        }

        public static void CloseLog(int numberOfErrors, int numberOfFilesSkipped, int numberOfFilesProcessed)
        {
            EventLogEntryType eventType;
            string messages;
            string results = "Successful " + numberOfFilesProcessed + " | Skipped " + numberOfFilesSkipped + " | Errors " + numberOfErrors;
            if (numberOfErrors != 0)
            {
                eventType = EventLogEntryType.Error;
                messages = "Process completed with errors.";
            }
            else
            {
                eventType = EventLogEntryType.Information;
                messages= "Process completed successfully.";
            }
            WriteToLog(messages);
            WriteToLog(results);
            WriteToEventLog("Log located at " + LogPath);
            WriteToEventLog(results);
            EventLog.WriteEntry("Image Resizer", string.Join("\r\n", EventLogMessages.ToArray()), eventType);
            results.WriteLine();
        }

        private static void CreateEventSourceIfItDoesntExist()
        {
            if (!EventLog.SourceExists("Image Resizer"))
                EventLog.CreateEventSource("Image Resizer", "Application");
        }
    }
}
