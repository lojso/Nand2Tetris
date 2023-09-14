namespace JackCompiler;

public static class Program
{
    public static void Main(string[] args)
    {
        var sourcesCollector = new SourcesCollector(args[0]);

        foreach (var source in sourcesCollector.GetSources())
        {
            var engine = new VmCompilationEngine(source.Content);
            
            var vmCode = engine.CompileClass();
            
            Console.WriteLine($"Saved vm code at: {SaveCode(source.Path, vmCode)}");
        }
        
    }

    private static string SaveCode(string filePath, string vmCode)
    {
        var directoryName = Path.GetDirectoryName(filePath);
        var listingFileName = Path.GetFileNameWithoutExtension(filePath);

        var outputFilePath = Path.Combine(directoryName, listingFileName) + ".vm";

        if (File.Exists(outputFilePath))
            File.Delete(outputFilePath);

        File.AppendAllText(outputFilePath, vmCode);

        return outputFilePath;
    }
}