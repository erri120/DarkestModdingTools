using System.IO;
using System.Text.Json;
using DarkestModdingTools.Core.GameFiles;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace DarkestModdingTools.Core.Parsers;

/// <summary>
/// Implements <see cref="IDataFileParser{TDataFile}"/> for <see cref="JsonDataFile"/>.
/// </summary>
[PublicAPI]
public class JsonDataFileParser : IDataFileParser<JsonDataFile>
{
    private static readonly Extension SupportedExtension = new(".json");
    private static readonly JsonDocumentOptions JsonDocumentOptions = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <inheritdoc/>
    public bool SupportsExtension(Extension extension) => extension.Equals(SupportedExtension);

    /// <inheritdoc/>
    public JsonDataFile ParseFile(Stream stream, RelativePath gamePath)
    {
        var jsonDocument = JsonDocument.Parse(stream, JsonDocumentOptions);
        return new JsonDataFile
        {
            GamePath = gamePath,
            JsonDocument = jsonDocument
        };
    }
}
