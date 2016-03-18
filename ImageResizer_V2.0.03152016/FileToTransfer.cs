using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace ImageResizer_V2._0._03152016
{
    class FileToTransfer
    {
        public string DestinationPath { get; private set; }
        public string SourcePath { get; set; }
        public string Extension { get; set; }
        public bool ExistsInDestination { get; set; }
        public DateTime LastWriteTime { get; set; }
        public ImageCodecInfo EncoderInfo { get; set; }
#if DEBUG
        protected const string PathPrefix = @"C:\AppTesting\acs\acs\firms\";
        protected const string PathSuffix = @"\images\jobs\";
#else   
        protected const string PathPrefix = @"\\schintranet\acs\acs\firms\";
        protected const string PathSuffix = @"\images\jobs\";
#endif

        public FileToTransfer(string sourcePath, string firmID)
        {
            // TODO: Get parent directory name from source, add it to destination path
            SourcePath = sourcePath;
            Extension = Path.GetExtension(SourcePath);
            DestinationPath = Path.Combine(PathPrefix + "firm" + firmID.ToString() + PathSuffix, Path.GetFileName(sourcePath));
            CheckDestination();
            ExistsInDestination = File.Exists(DestinationPath);
            LastWriteTime = File.GetLastWriteTime(SourcePath);
            EncoderInfo = GetEncoderInfo();
        }

        protected void CheckDestination()
        {
            if (!Directory.Exists(DestinationPath))
            {
                Log.WriteToLog("Directory not found: " + DestinationPath);
                Log.WriteToEventLog("Directory not found: " + DestinationPath, EventLogEntryType.Error);
            }
        }

        private ImageCodecInfo GetEncoderInfo()
        {
            if (!Extension.StartsWith("."))
                Extension = "." + Extension;
            string MIMEType = FileTypes.MIMETypes.TryGetValue(Extension, out MIMEType) ? MIMEType : "application/octet-stream";

            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < encoders.Length; i++)
                if (encoders[i].MimeType == MIMEType)
                    return encoders[i];
            return null;
        }
    }
}
