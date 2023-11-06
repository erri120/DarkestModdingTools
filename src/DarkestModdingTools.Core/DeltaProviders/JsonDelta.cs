using System.Text.Json.Nodes;
using JetBrains.Annotations;

namespace DarkestModdingTools.Core.DeltaProviders;

[PublicAPI]
public readonly record struct JsonDelta(string Path, Delta<JsonNode> Delta)
{
    public JsonDelta(JsonNode left, JsonNode right, DeltaKind kind = DeltaKind.Modified)
        : this(left.GetPath(), new Delta<JsonNode>(left, right, kind)) { }
}
