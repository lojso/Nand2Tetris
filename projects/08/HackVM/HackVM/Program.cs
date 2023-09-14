using System;
using System.Collections.Generic;
using System.IO;
using HackVM.Commands;

namespace HackVM
{
    class Program
    {
        private static Arguments _args;

        public static void Main(string[] args)
        {
            _args = new Arguments(args);
            var listing = new VMCodeParser(_args.Path);

            var commands = new List<VMCommand>();

            foreach (var codeline in listing.CodeListing())
                commands.Add(new VMCommand(codeline, listing.FileName));

            OutputByteCode(commands);
        }

        private static void OutputByteCode(List<VMCommand> commands)
        {
            var directoryName = Path.GetDirectoryName(_args.Path);
            var listingFileName = Path.GetFileNameWithoutExtension(_args.Path);
            var outputFile = Path.Combine(directoryName, listingFileName) + ".asm";

            var translator = new CodeTranslator();

            GetInitCode(translator);

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            File.AppendAllText(outputFile, GetInitCode(translator) + Environment.NewLine);

            foreach (var command in commands)
                File.AppendAllText(outputFile,
                    $"\\\\{command}" + Environment.NewLine +
                    translator.TranslateCommand(command) +
                    "\\\\TRANSLATION END" +
                    Environment.NewLine + Environment.NewLine);

            Console.WriteLine($"Saved asm code at: {outputFile}");
        }

        private static string GetInitCode(CodeTranslator codeTranslator) =>
            GetInitStackPointerAsmCode() +
            codeTranslator.TranslateCommand(GetLoadSystemCommand());

        private static VMCommand GetLoadSystemCommand() =>
            new VMCommand("call Sys.init 0", "Sys");

        private static string GetInitStackPointerAsmCode() =>
            $"@256{Environment.NewLine}" +
            $"D=A{Environment.NewLine}" +
            $"@SP{Environment.NewLine}" +
            $"M=D{Environment.NewLine}";
    }
}