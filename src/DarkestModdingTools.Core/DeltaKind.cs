using JetBrains.Annotations;

namespace DarkestModdingTools.Core;

[PublicAPI]
public enum DeltaKind
{
    None = 0,
    Added = 1,
    Modified = 2,
    Removed = 3
}
