using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace ImageResizer_V2._0._03152016
{
    class Program
    {
        private static Options options;
        private static Parser parser;

        private static string ServerName;
        private static string LocationCode;
        private static string Frim;
        private static string ImageSourcePath;
        private static string ImageDestinationPath;
        private static string ACSJobNumber;
        private static int MaxPictureSize;
        private static DateTime StartTime;
        static void Main(string[] args)
        {
            options = new Options();
            parser = new Parser();

            // TODO: Set default arguments

            if(parser.ParseArguments(args, options))
            {

            }
        }
    }
}
