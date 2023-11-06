using System.Linq;
using NexusMods.Paths;

namespace DarkestModdingTools.Core;

public static class ModComparer
{
    public static RelativePath[] GetSameFiles(Mod left, Mod right)
    {
        var keyIntersection = left.JsonDataFiles.Keys
            .Intersect(right.JsonDataFiles.Keys)
            .ToArray();

        return keyIntersection;
    }
}
