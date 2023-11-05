using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using DarkestModdingTools.Core.GameFiles;

namespace DarkestModdingTools.Core.DiffProviders;

public class JsonDataFileDiffProvider : IDiffProvider<JsonDataFile, List<JsonDelta>>
{
    public List<JsonDelta> Diff(JsonDataFile left, JsonDataFile right)
    {
        var deltas = Diff(left.Node, right.Node);
        return deltas;
    }

    private static List<JsonDelta> Diff(JsonNode leftStart, JsonNode rightStart)
    {
        var stack = new Stack<(JsonNode leftNode, JsonNode rightNode)>();
        stack.Push((leftStart, rightStart));

        var deltas = new List<JsonDelta>();

        while (stack.TryPop(out var pair))
        {
            var (leftNode, rightNode) = pair;
            if (leftNode is JsonObject leftObject && rightNode is JsonObject rightObject)
            {
                DiffJsonObjects(leftObject, rightObject, stack, deltas);
            } else if (leftNode is JsonArray leftArray && rightNode is JsonArray rightArray)
            {
                DiffJsonArrays(leftArray, rightArray, stack, deltas);
            } else if (leftNode is JsonValue leftValue && rightNode is JsonValue rightValue)
            {
                var optionalDelta = DiffJsonValues(leftValue, rightValue);
                if (!optionalDelta.HasValue) continue;
                deltas.Add(optionalDelta.Value);
            }
            else if (leftNode.GetType() != rightNode.GetType())
            {
                // same path, different value types
                throw new NotImplementedException();
            }
            else
            {
                throw new NotSupportedException($"Unknown type: {leftNode.GetType()} {rightNode.GetType()}");
            }
        }

        return deltas;
    }

    private static void DiffJsonObjects(
        JsonObject left,
        JsonObject right,
        Stack<ValueTuple<JsonNode, JsonNode>> stack,
        List<JsonDelta> deltas)
    {
        if (left.Count != right.Count)
            throw new NotImplementedException();

        foreach (var kv in left)
        {
            var (key, leftNode) = kv;
            if (leftNode is null) throw new NotImplementedException();

            if (!right.ContainsKey(key))
            {
                deltas.Add(new JsonDelta(leftNode.GetPath(), new Delta<JsonNode>(leftNode, Optional<JsonNode>.None, DeltaKind.Removed)));
                continue;
            }

            var rightNode = right[key];
            if (rightNode is null) throw new NotImplementedException();

            stack.Push((leftNode, rightNode));
        }

        foreach (var kv in right)
        {
            var (key, rightNode) = kv;
            if (rightNode is null) throw new NotImplementedException();
            if (left.ContainsKey(key)) continue;

            deltas.Add(new JsonDelta(rightNode.GetPath(), new Delta<JsonNode>(Optional<JsonNode>.None, rightNode, DeltaKind.Added)));
        }
    }

    private static void DiffJsonArrays(
        JsonArray left,
        JsonArray right,
        Stack<ValueTuple<JsonNode, JsonNode>> stack,
        List<JsonDelta> deltas)
    {
        if (left.Count != right.Count)
            throw new NotImplementedException();

        for (var i = 0; i < left.Count; i++)
        {
            var leftNode = left[i];
            var rightNode = right[i];

            if (leftNode is null && rightNode is null) continue;
            if (leftNode is null || rightNode is null)
            {
                throw new NotImplementedException();
            }

            stack.Push((leftNode, rightNode));
        }
    }

    private static Optional<JsonDelta> DiffJsonValues(JsonValue left, JsonValue right)
    {
        if (!left.TryGetValue(out JsonElement leftElement))
            throw new NotSupportedException($"{nameof(JsonValue)} doesn't contain a {nameof(JsonElement)}: {left.GetType()}");

        if (!right.TryGetValue(out JsonElement rightElement))
            throw new NotSupportedException($"{nameof(JsonValue)} doesn't contain a {nameof(JsonElement)}: {right.GetType()}");

        if (leftElement.ValueKind != rightElement.ValueKind)
        {
            // TODO: investigate if the engine cares about types or whether "2" == 2
            return new JsonDelta(left, right);
        }

        switch (leftElement.ValueKind)
        {
            case JsonValueKind.String:
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
                return leftElement.ValueKind switch
                {
                    JsonValueKind.String => string.Equals(
                        leftElement.GetString(),
                        rightElement.GetString(),
                        StringComparison.Ordinal)
                        ? Optional<JsonDelta>.None
                        : new JsonDelta(left, right),
                    JsonValueKind.Number when leftElement.TryGetInt64(out var leftInt64) &&
                                              rightElement.TryGetInt64(out var rightInt64) =>
                        leftInt64.Equals(rightInt64) ? Optional<JsonDelta>.None : new JsonDelta(left, right),
                    JsonValueKind.Number when leftElement.TryGetDouble(out var leftDouble) &&
                                              rightElement.TryGetDouble(out var rightDouble) =>
                        leftDouble.Equals(rightDouble) ? Optional<JsonDelta>.None : new JsonDelta(left, right),
                    JsonValueKind.Number => new JsonDelta(left, right),
                    _ => Optional<JsonDelta>.None
                };
            case JsonValueKind.Undefined:
            case JsonValueKind.Object:
            case JsonValueKind.Array:
            case JsonValueKind.Null:
            default:
                throw new NotSupportedException($"Unsupported value kind: {leftElement.ValueKind} at {left.GetPath()}");
        }
    }
}
