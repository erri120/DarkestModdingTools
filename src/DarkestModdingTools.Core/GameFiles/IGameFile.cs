using JetBrains.Annotations;
using NexusMods.Paths;

namespace DarkestModdingTools.Core.GameFiles;

/// <summary>
/// Represents a game file.
/// </summary>
[PublicAPI]
public interface IGameFile
{
    /// <summary>
    /// Gets the path to the file relative to the root directory of the game.
    /// </summary>
    public RelativePath GamePath { get; }
}
