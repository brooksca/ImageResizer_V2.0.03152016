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
            Log.BeginLog();
            NumberOfFilesProcessed = 0;
            NumberOfFilesSkipped = 0;
            NumberOfErrors = 0;

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (!Directory.Exists(options.Directory))
                    Directory.CreateDirectory(options.Directory);
                Location = new LocationFinder();
#if !DEBUG
                if (!Utilities.CheckDependencies())
                {
                    ("Schintranet not found on network. Are you sure you're connected to the IPS network?").WriteLine();
                    ("Exiting...").WriteLine();
                    Log.WriteToLog("Fatal error: Failed to ping schintranet");
                    Log.WriteToEventLog("Fatal error: Failed to ping schintranet", EventLogEntryType.Error);
                    Log.CloseWithErrors();
                    Environment.Exit(-1);
                }
#endif
                foreach (string directory in Directory.GetDirectories(options.Directory))
                    IterateOverFiles(directory);
            }
            Console.WriteLine();
            Log.CloseLog(NumberOfErrors, NumberOfFilesSkipped, NumberOfFilesProcessed);

#if DEBUG
            Utilities.PressAnyKeyToExit();
#endif
        }

        private static void IterateOverFiles(string directory)
        {
            IterateThroughDirectories(directory);
            if (Directory.GetFiles(directory).Length == 0)
            {
                ("No files found in directory: " + directory).WriteLine();
                Log.WriteToLog("Directory empty: " + directory);
                Log.WriteToEventLog("No files found in directory: " + directory);
            }
            else
            {
                "Beginning image resizing...".WriteLine();
                Log.WriteToLog("Beginning image resizing...");
                foreach (string file in Directory.GetFiles(directory))
                {
                    FileToTransfer thisFile = new FileToTransfer(file, Location.FirmID);
                    if (!options.ProcessOldFiles && thisFile.LastWriteTime < DateTime.Today.AddMonths(-1))
                    {
                        ("Skipping old file: " + thisFile.SourcePath).WriteLine();
                        Log.WriteToLog("Old file skipped (write date: " + thisFile.LastWriteTime + "): " + thisFile.SourcePath);
                        NumberOfFilesSkipped++;
                        continue;
                    }

                    if (FileTypes.ImageFileTypes.Contains(thisFile.Extension))
                    {
                        if (thisFile.ExistsInDestination)
                        {
                            ("File exists in destination: " + thisFile.SourcePath).WriteLine();
                            Log.WriteToLog("File exists in destination: " + thisFile.DestinationPath + " | (Destination: " + thisFile.DestinationPath + ")");
                            NumberOfFilesSkipped++;
                            continue;
                        }
                        else
                        {
                            ProcessImage(thisFile);
                        }
                    }
                }
            }
        }

        private static void IterateThroughDirectories(string directory)
        {
            foreach(string subDirectory in Directory.GetDirectories(directory))
                IterateOverFiles(subDirectory);
        }

        private static void ProcessImage(FileToTransfer file)
        {
            Log.WriteToLog("Processing image: " + file.SourcePath);
            Bitmap picture = (Bitmap)Image.FromFile(file.SourcePath);
            Size newSize;
            if (picture.Height <= options.PictureSize && picture.Width <= options.PictureSize)
                newSize = new Size(picture.Width, picture.Height);
            else
                newSize = new Size(options.PictureSize, options.PictureSize);

            Image ResizedImage = ImageResizer.Resize(picture, newSize);

            if(ResizedImage == null)
            {
                NumberOfErrors++;
                NumberOfFilesProcessed--;
                picture.Dispose();
                ResizedImage.Dispose();
                return;
            }

            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    ResizedImage.Save(memoryStream, file.EncoderInfo, encoderParameters);
                    var streamImage = Image.FromStream(memoryStream);
                    streamImage.Save(file.DestinationPath);
                }
                ResizedImage.Save(file.DestinationPath, file.EncoderInfo, encoderParameters);
                ("Image resize/move successful").WriteLine();
                Log.WriteToLog("Image resize/move successful: " + file.DestinationPath);
                NumberOfFilesProcessed++;
            }
            catch(Exception ex)
            {
                ("Image resize/move failed: " + ex.Message).WriteLine();
                Log.WriteToLog("Image resize/move failed: " + ex.Message + " (" + file.DestinationPath + ")");
                NumberOfErrors++;
            }
            finally
            {
                picture.Dispose();
                ResizedImage.Dispose();
            }
        }
    }
}
