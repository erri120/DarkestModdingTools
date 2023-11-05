using System;
using System.IO;
using System.Linq;
using DarkestModdingTools.Core.Parsers;
using FluentAssertions;
using NexusMods.Paths;
using Xunit;

namespace DarkestModdingTools.Core.Tests.Parsers;

public class JsonDataFileParserTests
{
    [Fact]
    public void Test_ParseFile()
    {
        var parser = new JsonDataFileParser();
        using var stream = GetTestFile("json/abbey.building.json");

        using var res = parser.ParseFile(stream, "campaign/town/buildings/abbey/abbey.building.json");
        res.HasException().Should().BeFalse();
        res.HasValue().Should().BeTrue();

        stream.Position.Should().Be(stream.Length);
    }

    [SkippableFact]
    public void Test_ParseFile_WithGameFiles()
    {
        Skip.IfNot(TryGetGameDirectory(out var gameDirectory), "Game path wasn't set");
        var jsonFiles = gameDirectory.EnumerateFiles(new Extension(".json"), recursive: true).ToArray();
        jsonFiles.Should().NotBeEmpty();

        var parser = new JsonDataFileParser();
        jsonFiles.Should().AllSatisfy(jsonFile =>
        {
            var gamePath = jsonFile.RelativeTo(gameDirectory);
            using var stream = jsonFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);

            using var res = parser.ParseFile(stream, gamePath);
            res.HasException().Should().BeFalse();
        });
    }

    private static bool TryGetGameDirectory(out AbsolutePath gameDirectory)
    {
        gameDirectory = default;

        var gameDirectoryString = Environment.GetEnvironmentVariable("DD_GAME_DIRECTORY", EnvironmentVariableTarget.Process);
        if (gameDirectoryString is null) return false;

        gameDirectory = FileSystem.Shared.FromUnsanitizedFullPath(gameDirectoryString);
        return true;
    }

    private static Stream GetTestFile(RelativePath path)
    {
        var file = FileSystem.Shared.GetKnownPath(KnownPath.CurrentDirectory).Combine("test-files").Combine(path);
        file.FileExists.Should().BeTrue($"File {file.ToString()} should exist!");
        return file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}
