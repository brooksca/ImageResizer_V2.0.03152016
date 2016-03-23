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

namespace ImageResizer_V2._0._03152016
{
    class Program
    {
        private static Options options;
        private static Parser parser;
        private static LocationFinder Location;
        private static int NumberOfFilesProcessed;
        private static int NumberOfFilesSkipped;
        private static int NumberOfErrors;

        static void Main(string[] args)
        {
            options = new Options();
            parser = new Parser();
            NumberOfFilesProcessed = 0;
            NumberOfFilesSkipped = 0;
            NumberOfErrors = 0;

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (!Directory.Exists(options.Directory))
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
#endif
                IterateThroughDirectories(options.Directory);
            }
            Console.WriteLine();
            HTMLLog.GenerateReportPanel(NumberOfErrors, NumberOfFilesSkipped, NumberOfFilesProcessed);
            HTMLLog.SaveAndClose();

            Utilities.PressAnyKeyToExit();
        }

        private static void IterateOverFiles(string directory)
        {
            IterateThroughDirectories(directory);
            if (Directory.GetFiles(directory).Length == 0)
            {
                return;
            }
            else
            {
                foreach (string file in Directory.GetFiles(directory)
                    .Where(f => File.GetLastWriteTime(f) > DateTime.Now.AddDays(-7)))
                {
                    Console.Write("\rResults: " + NumberOfFilesProcessed + " resized/moved | " + NumberOfFilesSkipped + " skipped | " + NumberOfErrors + " errors");
                    FileToTransfer thisFile = new FileToTransfer(file, Location.FirmID);
                    if (thisFile.LastWriteTime < DateTime.Today.AddDays(-14))
                    {
                        NumberOfFilesSkipped++;
                        continue;
                    }

                    if (FileTypes.ImageFileTypes.Contains(thisFile.Extension))
                    {
                        if (thisFile.ExistsInDestination)
                        {
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
        }

        private static void IterateThroughDirectories(string directory)
        {
            foreach (string subDirectory in Directory.GetDirectories(directory).Where(s => Directory.GetLastWriteTime(s) > DateTime.Now.AddDays(-14)))
                IterateOverFiles(subDirectory);
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
                resizedImage.Save(file.DestinationPath, file.EncoderInfo, encoderParameters);
                HTMLLog.AddToLog(new LogMessage()
                {
                    Message = "Image resize/move successful",
                    Time = DateTime.Now,
                    Type = MessageType.success
                }, file);
                NumberOfFilesProcessed++;
            }
            catch (Exception ex)
            {
                HTMLLog.AddToLog(new LogMessage()
                {
                    Message = "Failed to save file",
                    Time = DateTime.Now,
                    Type = MessageType.error,
                    Details = ex.Message
                }, file);
                NumberOfErrors++;
            }
            finally
            {
                resizedImage.Dispose();
            }
        }
    }
}
