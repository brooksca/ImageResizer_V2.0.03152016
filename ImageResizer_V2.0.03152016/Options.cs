using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace ImageResizer_V2._0._03152016
{
    class Options
    {
        [Option('m', "make-directory", Required = false, HelpText = "Creates the image directory if it doesn't exist.")]
        public bool CreateDirectory { get; set; }

        [Option('a', "all-files", Required = false, HelpText = "Processes files more than 30 days old (default: false)")]
        public bool ProcessOldFiles { get; set; }

        [Option('p', "pdf", Required = false, HelpText = "Processes PDFs when encountered (default: false")]
        public bool ProcessPDFs { get; set; }

        // TODO: Set default value to the default directory
        [Option('d', "directory", Required = false, HelpText = "Specifies the directory from which to process the images.",
#if DEBUG
            DefaultValue = @"C:\common\RSZ - ACS Job Photos\"
#else
            DefaultValue = @"D:\common\RSZ - ACS Job Photos\"
#endif
)]
        public string Directory { get; set; }

        [Option('s', "max-picture-size", Required = false, HelpText = "Sets the size that pictures will be reformatted to.", DefaultValue = 1000)]
        public int PictureSize { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("IPS Image Resizer", "Version 2.0.03152016"),
                Copyright = new CopyrightInfo("Allen Brooks", DateTime.Now.Year),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("-----------------------");
            help.AddPreOptionsLine("Usage: ImageResizer -m");
            help.AddOptions(this);
            return help;
        }
    }
}
