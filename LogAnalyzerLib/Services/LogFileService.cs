using LogAnalyzerLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogAnalyzerLib.Services
{
    public class LogFileService
    {
        public IEnumerable<LogFile> GetLogFiles(IEnumerable<string> directories)
        {
            if (directories.Count() < 1)
                throw new ArgumentException("At least one directory must be specified");

            var fileLocations = new List<string>();
            var logFiles = new List<LogFile>();
            foreach (var dir in directories)
            {
                if (!Directory.Exists(dir))
                    throw new DirectoryNotFoundException($"{dir} not valid");

                fileLocations.AddRange(Directory.GetFiles(dir));
            }

            foreach (var location in fileLocations)
            {
                var file = GetFile(location);
                if (file != null)
                    logFiles.Add(file);
            }

            return logFiles;
        }

        public IEnumerable<string> GetZipFiles(IEnumerable<string> directories)
        {
            if (directories.Count() < 1)
                throw new ArgumentException("At least one directory must be specified");

            var fileLocations = new List<string>();
            var zipFileNames = new List<string>();
            foreach (var dir in directories)
            {
                if (!Directory.Exists(dir))
                    throw new DirectoryNotFoundException($"{dir} not valid");

                fileLocations.AddRange(Directory.GetFiles(dir));
            }

            foreach (var location in fileLocations)
            {
                var file = GetFileInfo(location);
                if(file.Extension.Equals(".zip"))
                    zipFileNames.Add(file.FullName);
            }

            return zipFileNames;
        }

        public int CountFiles(IEnumerable<string> directories, bool getArchivesOnly = false)
        {
            var fileNames = new List<string>();
            var logFiles = new List<LogFile>();
            foreach (var dir in directories)
            {
                if (!Directory.Exists(dir))
                    throw new DirectoryNotFoundException($"{dir} not valid");

                fileNames.AddRange(Directory.GetFiles(dir));
            }
            return fileNames.Count();
        }

        public LogFile GetFile(string fileLocation)
        {
            if (!File.Exists(fileLocation))
                //throw new FileNotFoundException($"No file found in location: {fileLocation}");
                return null;

            var file = new FileInfo(fileLocation);
            if (file.Extension != ".log")
                //throw new FileLoadException("Identified file is not a log file");
                return null;

            return new LogFile
            {
                File = file,
                Name = file.Name,
                Size = file.Length * 1024,
                FileLocation = fileLocation,
                Date = file.LastWriteTime
            };
        }

        public FileInfo GetFileInfo(string fileLocation)
        {
            if (!File.Exists(fileLocation))
                return null;

            return new FileInfo(fileLocation);
        }

        public string GetLogFileString(string fileLocation)
        {
            string resp = string.Empty;
            using (var stream = new StreamReader(fileLocation))
            {
                for (var i = 0; i < 20; i++)
                {
                    resp += stream.ReadLine();
                }
            }
            return resp;
        }

        public IEnumerable<LogFile> SearchFilesByTime(DateTime fromDate, DateTime toDate, IEnumerable<string> directories, bool getArchivesOnly = false)
        {
            if (directories.Count() < 1)
                throw new ArgumentException("At least one directory must be specified");

            if (fromDate > toDate)
                throw new ArgumentException("FromDate cannot be greater than ToDate");

            var fileLocations = new List<string>();
            var logFiles = new List<LogFile>();
            foreach (var dir in directories)
            {

                if (!Directory.Exists(dir))
                    throw new DirectoryNotFoundException($"{dir} not valid");

                fileLocations.AddRange(Directory.GetFiles(dir));
            }

            foreach (var location in fileLocations)
            {
                var file = GetFile(location);
                if (file != null)
                    logFiles.Add(file);
                if (file.Date > fromDate && file.Date < toDate)
                {
                    if ((getArchivesOnly && !file.File.Extension.Equals("zip")) ||
                        (!getArchivesOnly && file.File.Extension.Equals("zip")))
                        continue;
                    logFiles.Add(file);
                }
            }

            return logFiles;
        }

        public IEnumerable<LogFile> SearchFilesBySize(long sizeFrom, long sizeTo, IEnumerable<string> directories)
        {
            if (directories.Count() < 1)
                throw new ArgumentException("At least one directory must be specified");

            if (sizeFrom > sizeTo)
                throw new ArgumentException("FromSize cannot be greater than ToSize");

            var fileLocations = new List<string>();
            var logFiles = new List<LogFile>();
            foreach (var dir in directories)
            {
                if (!Directory.Exists(dir))
                    throw new DirectoryNotFoundException($"{dir} not valid");

                fileLocations.AddRange(Directory.GetFiles(dir));
            }

            foreach (var location in fileLocations)
            {
                var file = GetFile(location);
                if (file.Size >= sizeFrom * 1048576   && file.Size <= sizeTo * 1048576)
                    logFiles.Add(file);
            }

            return logFiles;
        }

        public void DeleteFiles(IEnumerable<LogFile> files)
        {
            if (files.Count() < 1)
                throw new ArgumentException("At least one file must be selected for deletion");

            foreach (var file in files)
            {
                if (File.Exists(file.FileLocation))
                    File.Delete(file.FileLocation);
            }
        }

        public (bool isSuccessful, string message) ZipFiles(IEnumerable<string> directories, DateTime fromDate, DateTime toDate)
        {
            if (directories.Count() < 1)
                throw new ArgumentException("At least one directory must be specified");

            foreach (var directory in directories)
            {
                var filesToZip = SearchFilesByTime(fromDate, toDate, new List<string> { directory });

                if (filesToZip.Count() < 1)
                    return (false, "No files available to zip");

                var earliestFile = filesToZip.Min(x => x.Date);
                var latestFile = filesToZip.Max(x => x.Date);
                using (ZipArchive zip = ZipFile.Open($@"{directory}\\{earliestFile.Date:dd_MM_yyyy}-{latestFile.Date:dd_MM_yyyy}.zip", ZipArchiveMode.Create))
                {
                    foreach (var file in filesToZip)
                    {
                        zip.CreateEntryFromFile($"{file.FileLocation}", $"{file.File.Name}", CompressionLevel.Optimal);
                    }
                }

                DeleteFiles(filesToZip);
            }
            return (true, "Logs zipped successfully");
        }

        public void DeleteZipsByDateRange(string directory, DateTime fromDate, DateTime toDate)
        {
            var zipsToDelete = SearchFilesByTime(fromDate, toDate, new List<string> { directory }, true);
            DeleteFiles(zipsToDelete);
        }

        public bool UploadToServer(IEnumerable<LogFile> files)
        {
            throw new NotImplementedException();
        }

    }
}
