using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLib.Models
{
    public class LogItem
    {
        public string NameSpace { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string VersionNumber { get; set; }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public int LogFileId { get; set; }
    }
}
