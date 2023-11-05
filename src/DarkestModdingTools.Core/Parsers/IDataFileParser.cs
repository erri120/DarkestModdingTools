using System.IO;
using DarkestModdingTools.Core.GameFiles;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace DarkestModdingTools.Core.Parsers;

/// <summary>
/// Represents a data file parser.
/// </summary>
[PublicAPI]
public interface IDataFileParser<TDataFile>
    where TDataFile : class, IDataFile
{
    /// <summary>
    /// Tries to parse the given <see cref="Stream"/> into <see cref="TDataFile"/>.
    /// </summary>
    /// <remarks>
    /// The provided <see cref="Stream"/> will be disposed by the caller.
    /// </remarks>
    public ParserResult<TDataFile> ParseFile(Stream stream, RelativePath gamePath);
}
