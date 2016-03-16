using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace ImageResizer_V2._0._03152016
{
    class ImageResizer
    {

        public static Image Resize(Image image, Size size)
        {

            try
            {
                int sourceWidth = image.Width;
                int sourceHeight = image.Height;

                float WidthScaling = ((float)size.Width / (float)sourceWidth);
                float HeightScaling = ((float)size.Height / (float)sourceHeight);
                float ScalingValue = HeightScaling < WidthScaling ? HeightScaling : WidthScaling;

                int destinationWidth = (int)(sourceWidth * ScalingValue);
                int destinationHeight = (int)(sourceHeight * ScalingValue);

                Bitmap bitmap = new Bitmap(destinationWidth, destinationHeight);
                Graphics graphics = Graphics.FromImage((Image)bitmap);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, destinationWidth, destinationHeight);
                graphics.Dispose();
                return (Image)bitmap;
            }
            catch (Exception ex)
            {
                ("Image resize failed. Moving to next file.").WriteLine();
                Log.WriteToLog("Image resizing failed: " + ex.Message);
                Log.WriteToEventLog("Image resizing failed: " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }
    }
}
