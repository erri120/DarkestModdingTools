using System.Collections.Generic;
using System.IO;
using DarkestModdingTools.Core.Parsers;
using FluentAssertions;
using NexusMods.Paths;
using Xunit;

namespace DarkestModdingTools.Core.Tests.Parsers;

public class JsonDataFileParserTests
{
    [Theory]
    [MemberData(nameof(TestData_SupportsExtensions))]
    public void Test_SupportsExtension(Extension input, bool expected)
    {
        var parser = new JsonDataFileParser();
        var actual = parser.SupportsExtension(input);
        actual.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestData_SupportsExtensions() => new[]
    {
        new object[]{ new Extension(".json"), true },
        new object[]{ new Extension(".JSON"), true },
        new object[]{ new Extension(".txt"), false }
    };

    [Fact]
    public void Test_ParseFile()
    {
        var parser = new JsonDataFileParser();
        using var stream = GetTestFile("json/abbey.building.json");

        // ReSharper disable once AccessToDisposedClosure
        var act = () => parser.ParseFile(stream, "campaign/town/buildings/abbey/abbey.building.json");
        act.Should().NotThrow();

        stream.Position.Should().Be(stream.Length);
    }

    private static Stream GetTestFile(RelativePath path)
    {
        var file = FileSystem.Shared.GetKnownPath(KnownPath.CurrentDirectory).Combine("test-files").Combine(path);
        file.FileExists.Should().BeTrue($"File {file.ToString()} should exist!");
        return file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}
