using System.Collections.Generic;
using System.IO;
using System.Linq;
using DarkestModdingTools.Core.GameFiles;
using DarkestModdingTools.Core.Parsers;
using NexusMods.Paths;

namespace DarkestModdingTools.Core;

public static class ModLoader
{
    public static Mod LoadModFromDirectory(AbsolutePath modDirectory)
    {
        var projectFile = modDirectory.Combine("project.xml");
        if (!projectFile.FileExists) throw new FileNotFoundException($"Required file doesn't exist: {projectFile}");

        // var modfilesFile = modDirectory.Combine("modfiles.txt");
        // var previewIconFile = modDirectory.Combine("preview_icon.png");

        var modFiles = modDirectory
            .EnumerateFiles(recursive: true)
            .ToArray();

        var parser = new JsonDataFileParser();
        var jsonDataFiles = new Dictionary<RelativePath, JsonDataFile>();

        foreach (var modFile in modFiles)
        {
            using var stream = modFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            var res = parser.ParseFile(stream, modFile.RelativeTo(modDirectory));
            if (res.HasException()) res.ThrowException();
            if (res == ParserResult<JsonDataFile>.Unsupported) continue;

            var value = res.GetValue();
            jsonDataFiles.Add(value.GamePath, value);
        }

        return new Mod
        {
            Directory = modDirectory,
            JsonDataFiles = jsonDataFiles
        };
    }
}
