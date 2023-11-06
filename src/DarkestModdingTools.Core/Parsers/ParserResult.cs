using System;
using System.Diagnostics.CodeAnalysis;
using DarkestModdingTools.Core.GameFiles;
using JetBrains.Annotations;

namespace DarkestModdingTools.Core.Parsers;

[PublicAPI]
public readonly struct ParserResult<TDataFile> : IEquatable<ParserResult<TDataFile>>, IDisposable
    where TDataFile : class, IDataFile
{
    private readonly TDataFile? _value;
    private readonly Exception? _exception;

    public static readonly ParserResult<TDataFile> Unsupported = new();

    public ParserResult()
    {
        _value = null;
        _exception = null;
    }

    private ParserResult(TDataFile value)
    {
        _value = value;
        _exception = null;
    }

    private ParserResult(Exception exception)
    {
        _value = null;
        _exception = exception;
    }

    public bool HasValue() => _value is not null;
    public bool HasException() => _exception is not null;

    public bool TryGetValue([NotNullWhen(true)] out TDataFile? value)
    {
        value = _value;
        return value is not null;
    }

    public bool TryGetException([NotNullWhen(true)] out Exception? exception)
    {
        exception = _exception;
        return exception is not null;
    }

    public TDataFile GetValue()
    {
        if (_value is null) throw new InvalidOperationException();
        return _value;
    }

    public Exception GetException()
    {
        if (_exception is null) throw new InvalidOperationException();
        return _exception;
    }

    [DoesNotReturn]
    [ContractAnnotation("=> halt")]
    public void ThrowException() => throw GetException();

    public static ParserResult<TDataFile> FromValue(TDataFile value)
    {
        return new ParserResult<TDataFile>(value);
    }

    public static ParserResult<TDataFile> FromException(Exception exception)
    {
        return new ParserResult<TDataFile>(exception);
    }

    public static implicit operator ParserResult<TDataFile>(TDataFile dataFile) => FromValue(dataFile);
    public static implicit operator ParserResult<TDataFile>(Exception e) => FromException(e);

    public bool Equals(ParserResult<TDataFile> other)
    {
        return ReferenceEquals(_value, other._value) && ReferenceEquals(_exception, other._exception);
    }

    public override bool Equals(object? obj)
    {
        return obj is ParserResult<TDataFile> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_value, _exception);
    }

    public static bool operator ==(ParserResult<TDataFile> left, ParserResult<TDataFile> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ParserResult<TDataFile> left, ParserResult<TDataFile> right)
    {
        return !(left == right);
    }

    public void Dispose()
    {
        _value?.Dispose();
    }
}
