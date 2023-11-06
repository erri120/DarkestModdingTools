using System.Collections.Generic;
using DarkestModdingTools.Core.GameFiles;

namespace DarkestModdingTools.Core.DeltaProviders;

public interface IDeltaProvider<in TDataFile, TOut>
    where TDataFile : class, IDataFile
    where TOut : notnull
{
    public List<TOut> Diff(TDataFile left, TDataFile right);
}
