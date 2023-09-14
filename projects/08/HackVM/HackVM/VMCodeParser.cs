using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HackVM.Commands
{
    public class VMCodeParser
    {
        private readonly List<string> _filePathes = new List<string>();

        public VMCodeParser(string sourcePath)
        {
            if (Directory.Exists(sourcePath))
            {
                AddDirectoryFiles(sourcePath);
                return;
            }

            if (File.Exists(sourcePath))
            {
                AddSingleFile(sourcePath);
                return;
            }

            throw new ArgumentException($"Cant find source files at {sourcePath}");
        }

        public string FileName { get; private set; }

        public IEnumerable<string> CodeListing() =>
            _filePathes.SelectMany(ParseSourceFile);

        private void AddSingleFile(string sourcePath)
        {
            if (sourcePath.EndsWith(".vm") == false)
                throw new ArgumentException($"File at {sourcePath} shoud have '.vm' extension");

            _filePathes.Add(sourcePath);
        }

        private void AddDirectoryFiles(string directoryPath)
        {
            foreach (var path in Directory.EnumerateFiles(directoryPath).Where(file => file.EndsWith(".vm")))
                _filePathes.Add(path);
        }

        private IEnumerable<string> ParseSourceFile(string filePath)
        {
            using var file = new StreamReader(filePath);
            FileName = Path.GetFileName(filePath);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                var parsedLine = line.Trim();
                parsedLine = RemoveComments(parsedLine);
                if (parsedLine.Length != 0)
                    yield return parsedLine;
            }
        }

        private string RemoveComments(string line)
        {
            var commentSymbol = "//";
            var firstOccurence = line.IndexOf(commentSymbol, StringComparison.Ordinal);
            if (firstOccurence == -1)
                return line;
            return line.Substring(0, firstOccurence);
        }
    }
}