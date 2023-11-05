using DarkestModdingTools.Core.GameFiles;

namespace DarkestModdingTools.Core.DiffProviders;

public interface IDiffProvider<in TDataFile, out TOut>
    where TDataFile : class, IDataFile
    where TOut : class
{
    public TOut? Diff(TDataFile left, TDataFile right);
}
