using System.Text.Json.Nodes;

namespace DarkestModdingTools.Core;

internal static class JsonExtensions
{
    public static string GetPathWithKey(this JsonObject jsonObject, string key) => $"{jsonObject.GetPath()}.{key}";

    public static string GetPathWithIndex(this JsonArray jsonArray, int index) => $"{jsonArray.GetPath()}[{index}]";
}
