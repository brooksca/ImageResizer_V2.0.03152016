using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Runtime.InteropServices;

namespace ImageResizer_V2._0._03152016
{
    class Program
    {
        private static Options options;
        private static Parser parser;
        private static LocationFinder Location;
        private static int NumberOfFilesSuccessful;
        private static int NumberOfFilesSkipped;
        private static int NumberOfErrors;
        private static int TotalNumberOfFilesToBeProcessed;
        private static int TotalNumberOfFilesProcessed;
        private static IEnumerable<string> DirectoriesToScan;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler exitHandler, bool add);
        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;
        enum CtrlType { CTRL_C_EVENT = 0, CTRL_BREAK_EVENT = 1, CTRL_CLOSE_EVENT = 2, CTRL_LOGOFF_EVENT = 5, CTRL_SHUTDOWN_EVENT = 6 }

        private static bool Handler(CtrlType sig)
        {
            HTMLLog.AddToLog(new LogMessage()
            {
                Message = "User shutdown",
                Time = DateTime.Now,
                Type = MessageType.error,
                Details = "User cancelled operation"
            });
            "Updating log before exit...".WriteLine();
            HTMLLog.GenerateReportPanel(NumberOfErrors, NumberOfFilesSkipped, NumberOfFilesSuccessful);
            HTMLLog.SaveAndClose();
            Thread.Sleep(2000);
            Environment.Exit(-1);
            return true;
        }

        static void Main(string[] args)
        {
            options = new Options();
            parser = new Parser();
            NumberOfFilesSuccessful = 0;
            NumberOfFilesSkipped = 0;
            NumberOfErrors = 0;

            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                GetFileCount();
                if (!Directory.Exists(options.Directory) && options.CreateDirectory)
                    Directory.CreateDirectory(options.Directory);
                HTMLLog.BeginLog();
                Location = new LocationFinder();
                HTMLLog.Location = Location;
                
#if !DEBUG
                if (!Utilities.CheckDependencies())
                {
                    ("Schintranet not found on network.").WriteLine();
                    ("Exiting...").WriteLine();
                    HTMLLog.AddToLog(new LogMessage()
                    {
                        Message = "Fatal error",
                        Time = DateTime.Now,
                        Type = MessageType.error,
                        Details = "Couldn't ping schintranet"
                    });
                    HTMLLog.GenerateReportPanel(1, 0, 0);
                    HTMLLog.SaveAndClose();
                    Thread.Sleep(2000);
                    Environment.Exit(-1);
                }
                else
                {
                    HTMLLog.AddToLog(new LogMessage()
                    {
                        Message = "Schintranet found",
                        Time = DateTime.Now,
                        Type = MessageType.success,
                        Details = "Successfully pinged schintranet"
                    });
                }
#endif
                IterateThroughDirectories(options.Directory);
            }

            Console.WriteLine();
            HTMLLog.GenerateReportPanel(NumberOfErrors, NumberOfFilesSkipped, NumberOfFilesSuccessful);
            HTMLLog.SaveAndClose();

#if DEBUG
            Utilities.PressAnyKeyToExit();
            Process.Start(@"C:\ImageResizer\ImageResizeLog.html");
#endif
        }

        private static void GetFileCount()
        {
            DirectoriesToScan = from d in Directory.GetDirectories(options.Directory)
                                where DateTime.Compare(DateTime.Now.AddDays(-14), File.GetLastWriteTime(d)) < 0
                                select d;
            TotalNumberOfFilesToBeProcessed = 0;
            foreach (string subDirectory in DirectoriesToScan)
                TotalNumberOfFilesToBeProcessed += Directory.GetFiles(subDirectory).Length;
        }

        private static void IterateOverFiles(string directory)
        {
            foreach (string subDirectory in Directory.GetDirectories(directory))
                IterateThroughDirectories(subDirectory);
            if (Directory.GetFiles(directory).Length == 0)
                return;
            else
                foreach (string file in (from f in Directory.GetFiles(directory)
                                         where DateTime.Compare(DateTime.Now.AddDays(-14), File.GetLastWriteTime(f)) < 0
                                         select f))
                {
                    Console.Write("\rResults: " + NumberOfFilesSuccessful + " resized/moved | " + NumberOfFilesSkipped + " skipped | " + NumberOfErrors + " errors | " + Math.Round(((double)((double)TotalNumberOfFilesProcessed / (double)TotalNumberOfFilesToBeProcessed * 100)), 1) + "%");
                    FileToTransfer thisFile = new FileToTransfer(file, Location.FirmID);
                    if (FileTypes.ImageFileTypes.Contains(thisFile.Extension.ToLower()))
                    {
                        if (thisFile.ExistsInDestination)
                        {
                            TotalNumberOfFilesProcessed++;
                            NumberOfFilesSkipped++;
                            continue;
                        }
                        else
                        {
                            Image ResizedImage = ResizeImage(thisFile);
                            SaveImage(ResizedImage, thisFile);
                        }
                    }
                }
        }

        private static void IterateThroughDirectories(string parentDirectory)
        {
            IEnumerable<string> subDirectories = from d in Directory.GetDirectories(parentDirectory)
                                                 where DateTime.Compare(DateTime.Now.AddDays(-14), File.GetLastWriteTime(d)) < 0
                                                 select d;

            foreach (string subDirectory in subDirectories)
            {
                string destinationDirectory = Path.Combine(@"\\schintranet\acs\acs\firms\firm" + Location.FirmID + @"\images\jobs\", new DirectoryInfo(subDirectory).Name);
                Console.Write("\rResults: " + NumberOfFilesSuccessful + " resized/moved | " + NumberOfFilesSkipped + " skipped | " + NumberOfErrors + " errors | " + Math.Round(((double)((double)TotalNumberOfFilesProcessed / (double)TotalNumberOfFilesToBeProcessed * 100)), 1) + "%");
                if (!Directory.Exists(destinationDirectory))
                {
                    HTMLLog.AddToLog(new LogMessage()
                    {
                        Message = "Directory not found in destination",
                        Time = DateTime.Now,
                        Type = MessageType.warning,
                        Details = destinationDirectory
                    });
                    TotalNumberOfFilesProcessed += Directory.GetFiles(subDirectory).Length;
                    continue;
                }
                else
                {
                    IterateOverFiles(subDirectory);
                }
            }
        }

        private static Image ResizeImage(FileToTransfer file)
        {
            try
            {
                Bitmap picture = (Bitmap)Image.FromFile(file.SourcePath);
                Size newSize;
                if (picture.Height <= options.PictureSize && picture.Width <= options.PictureSize)
                    newSize = new Size(picture.Width, picture.Height);
                else
                    newSize = new Size(options.PictureSize, options.PictureSize);
                return ImageResizer.Resize(picture, newSize);             
            }
            catch (Exception ex)
            {
                NumberOfErrors++;
                TotalNumberOfFilesProcessed++;
                HTMLLog.AddToLog(new LogMessage()
                {
                    Message = "Error resizing image",
                    Time = DateTime.Now,
                    Type = MessageType.error,
                    Details = ex.Message
                }, file);
                return null;
            }
        }

        private static void SaveImage(Image resizedImage, FileToTransfer file)
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    resizedImage.Save(memoryStream, file.EncoderInfo, encoderParameters);
                    var streamImage = Image.FromStream(memoryStream);
                    streamImage.Save(file.DestinationPath);
                }
                NumberOfFilesSuccessful++;
                TotalNumberOfFilesProcessed++;
            }
            catch (Exception ex)
            {
                HTMLLog.AddToLog(new LogMessage()
                {
                    Message = "Image resize/move failed",
                    Time = DateTime.Now,
                    Type = MessageType.error,
                    Details = ex.Message
                }, file);
                NumberOfErrors++;
                TotalNumberOfFilesProcessed++;
            }
            finally
            {
                resizedImage.Dispose();
            }
        }
    }
}
