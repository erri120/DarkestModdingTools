using System;
using System.IO;
using FluentAssertions;
using NexusMods.Paths;

namespace DarkestModdingTools.Core.Tests;

public static class TestHelpers
{
    public static bool TryGetGameDirectory(out AbsolutePath gameDirectory)
    {
        gameDirectory = default;

        var gameDirectoryString = Environment.GetEnvironmentVariable("DD_GAME_DIRECTORY", EnvironmentVariableTarget.Process);
        if (gameDirectoryString is null) return false;

        gameDirectory = FileSystem.Shared.FromUnsanitizedFullPath(gameDirectoryString);
        return gameDirectory.DirectoryExists();
    }

    public static bool TryGetWorkshopDirectory(out AbsolutePath workshopDirectory)
    {
        workshopDirectory = default;

        var workshopDirectoryString = Environment.GetEnvironmentVariable("DD_WORKSHOP_DIRECTORY", EnvironmentVariableTarget.Process);
        if (workshopDirectoryString is null) return false;

        workshopDirectory = FileSystem.Shared.FromUnsanitizedFullPath(workshopDirectoryString);
        return workshopDirectory.DirectoryExists();
    }

    public static Stream GetTestFile(RelativePath path)
    {
        var file = FileSystem.Shared.GetKnownPath(KnownPath.CurrentDirectory).Combine("test-files").Combine(path);
        file.FileExists.Should().BeTrue($"File {file.ToString()} should exist!");
        return file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}
