using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
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

    private static readonly JsonNodeOptions JsonNodeOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonDocumentOptions JsonDocumentOptions = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <inheritdoc/>
    public ParserResult<JsonDataFile> ParseFile(Stream stream, RelativePath gamePath)
    {
        if (!gamePath.Extension.Equals(SupportedExtension))
            return ParserResult<JsonDataFile>.Unsupported;

        try
        {
            var node = JsonNode.Parse(stream, JsonNodeOptions, JsonDocumentOptions);
            var res = new JsonDataFile
            {
                GamePath = gamePath,
                Node = node!
            };

            return res;
        }
        catch (JsonException e)
        {
            if (e is { LineNumber: 0, BytePositionInLine: 0 })
                return ParserResult<JsonDataFile>.Unsupported;
            return e;
        }
    }
}
