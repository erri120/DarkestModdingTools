using System.Text.Json.Nodes;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace DarkestModdingTools.Core.GameFiles;

/// <summary>
/// Represents a JSON game file.
/// </summary>
[PublicAPI]
public sealed record JsonDataFile : IDataFile
{
    /// <inheritdoc/>
    public required RelativePath GamePath { get; init; }

    public required JsonNode Node { get; init; }

    /// <inheritdoc/>
    public void Dispose() { }
}
