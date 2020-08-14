using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLib.Models
{
    public class LogFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime Date { get; set; }
        public FileInfo File { get; set; }
        public string FileLocation { get; set; }
    }
}
