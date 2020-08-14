using LogAnalyzerLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzerLib.Services
{
    public class LogItemService
    {
        public IEnumerable<LogItem> GetLogItems(LogFile file)
        {

            if (file.File.Directory.Name == "AWIErrors")
            {
                //throw new ArgumentException("You must select a folder from AWIErrors folder");

                string logString = File.ReadAllText(file.FileLocation);
                var logItems = new List<LogItem>();

                var logItemsString = logString.Trim().Split(new string[] { "__________________________" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var itemString in logItemsString)
                {
                    try
                    {
                        var logItem = new LogItem();
                        var Delimiter1 = itemString.Trim().IndexOf(' ');
                        var Delimiter2 = itemString.Trim().IndexOf(' ', Delimiter1 + 1);
                        var Delimiter3 = itemString.Trim().IndexOf(" : ", Delimiter2 + 1);
                        var Delimiter4 = itemString.Trim().IndexOf("------>", Delimiter3 + 1);
                        logItem.DateTime = DateTime.ParseExact(itemString.Trim().Substring(0, Delimiter1), "dd.MM.yyyy", null);
                        //var time = itemString.Trim().Substring(Delimiter1, Delimiter2);
                        logItem.NameSpace = itemString.Trim().Substring(Delimiter2 + 1, Delimiter3 - Delimiter2 - 1);
                        logItem.Level = itemString.Trim().Substring(Delimiter3 + 3, Delimiter4 - Delimiter3 - 3);
                        logItem.Message = itemString.Trim().Substring(Delimiter4 + 7);

                        logItems.Add(logItem);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                return logItems;
            }

            else if(file.File.Directory.Name == "Loggings")
            {
                string logString = File.ReadAllText(file.FileLocation);
                var logItems = new List<LogItem>();

                var logItemsString = logString.Trim().Split(new string[] { "__________________________" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var itemString in logItemsString)
                {
                    try
                    {
                        var logItem = new LogItem();
                        var Delimiter1 = itemString.Trim().IndexOf(' ');
                        var Delimiter2 = itemString.Trim().IndexOf(' ', Delimiter1 + 1);
                        var Delimiter3 = itemString.Trim().IndexOf("------>", Delimiter2 + 1);
                        var Delimiter4 = itemString.Trim().IndexOf(" : ", Delimiter3 + 1);
                        logItem.DateTime = DateTime.ParseExact(itemString.Trim().Substring(0, Delimiter1), "dd.MM.yyyy", null);
                        //var time = itemString.Trim().Substring(Delimiter1, Delimiter2);
                        logItem.NameSpace = itemString.Trim().Substring(Delimiter2 + 1, Delimiter3 - Delimiter2 - 1);
                        logItem.Level = itemString.Trim().Substring(Delimiter3 + 7, Delimiter4 - Delimiter3 - 7);
                        logItem.Message = itemString.Trim().Substring(Delimiter4 + 3);

                        logItems.Add(logItem);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                return logItems;
            }
            else if (file.File.Directory.Name == "AmadeoLogs")
            {
                string logString = File.ReadAllText(file.FileLocation);
                var logItems = new List<LogItem>();

                var logItemsString = logString.Trim().Split(new string[] { "__________________________" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var itemString in logItemsString)
                {
                    try
                    {
                        var logItem = new LogItem();
                        var Delimiter1 = itemString.Trim().IndexOf(' ');
                        var Delimiter2 = itemString.Trim().IndexOf(' ', Delimiter1 + 1);
                        var Delimiter3 = itemString.Trim().IndexOf("------>", Delimiter2 + 1);
                        var Delimiter4 = itemString.Trim().IndexOf(" : ", Delimiter3 + 1);
                        logItem.DateTime = DateTime.ParseExact(itemString.Trim().Substring(0, Delimiter1), "dd.MM.yyyy", null);
                        //var time = itemString.Trim().Substring(Delimiter1, Delimiter2);
                        logItem.NameSpace = itemString.Trim().Substring(Delimiter2 + 1, Delimiter3 - Delimiter2 - 1);
                        logItem.Level = itemString.Trim().Substring(Delimiter3 + 7, Delimiter4 - Delimiter3 - 7);
                        logItem.Message = itemString.Trim().Substring(Delimiter4 + 3);

                        logItems.Add(logItem);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                return logItems;
            }
            throw new ArgumentException("You must select a folder from Log folder");
        }

        public long CountUniqueErrors(LogFile file)
        {
            return GetLogItems(file)
                    //.Where(x => x.Level.ToLower() == "error")
                    .GroupBy(x => x.Message)
                    .Count();
        }

        public long CountUniqueErrors(string fileLocation)
        {
            return CountUniqueErrors(new LogFileService().GetFile(fileLocation));
        }

        public long CountDuplicateErrors(LogFile file)
        {
            long totalCount = 0;
            var duplicates = GetLogItems(file)
                     // .Where(x => x.Level.ToLower() == "error")
                      .GroupBy(x => x.Message)
                      .Where(g => g.Count() > 1);
            foreach (var error in duplicates)
            {
                 totalCount += error.Count() - 1;
            }
            return totalCount;
        }

        public long CountDuplicateErrors(string fileLocation)
        {
            return CountDuplicateErrors(new LogFileService().GetFile(fileLocation));
        }
    }
}
