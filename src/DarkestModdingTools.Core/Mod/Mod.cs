using System.Collections.Generic;
using DarkestModdingTools.Core.GameFiles;
using NexusMods.Paths;

namespace DarkestModdingTools.Core;

public record Mod
{
    public required AbsolutePath Directory { get; init; }

    public required Dictionary<RelativePath, JsonDataFile> JsonDataFiles { get; init; }
}
