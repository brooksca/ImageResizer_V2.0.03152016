using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer_V2._0._03152016
{
    public enum MessageType { Error, Information }
    class LogMessage
    {
        public string Message { get; set; }
        public MessageType Type { get; set; }
        public DateTime Time { get; set; }
        public string Task { get; set; }
        public FileToTransfer File { get; set; }
    }
}
