using System;
using JetBrains.Annotations;

namespace DarkestModdingTools.Core.GameFiles;

/// <summary>
/// Represents an <see cref="IGameFile"/> that contains data.
/// </summary>
[PublicAPI]
public interface IDataFile : IGameFile, IDisposable { }


