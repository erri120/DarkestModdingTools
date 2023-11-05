using System.Diagnostics;
using JetBrains.Annotations;

namespace DarkestModdingTools.Core;

[PublicAPI]
public readonly record struct Delta<TValue>
    where TValue : notnull
{
    public readonly Optional<TValue> Previous;
    public readonly Optional<TValue> Current;
    public readonly DeltaKind Kind;

    public Delta(Optional<TValue> previous, Optional<TValue> current, DeltaKind kind)
    {
        Previous = previous;
        Current = current;
        Kind = kind;

        if (kind == DeltaKind.Added)
            Debug.Assert(!previous.HasValue && current.HasValue);
        if (kind == DeltaKind.Removed)
            Debug.Assert(previous.HasValue && !current.HasValue);
        if (kind == DeltaKind.Modified)
            Debug.Assert(previous.HasValue && current.HasValue);
    }
}
