using System;
using System.Text.Json;
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

    public required JsonDocument JsonDocument { get; init; }

    /// <inheritdoc/>
    public void Dispose()
    {
        JsonDocument.Dispose();
    }
}
