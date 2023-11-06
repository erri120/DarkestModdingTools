using System;
using System.Linq;
using DarkestModdingTools.Core.DeltaProviders;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DarkestModdingTools.Core.Tests;

public class ModTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    public ModTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [SkippableFact]
    public void Test_ModLoader()
    {
        Skip.IfNot(TestHelpers.TryGetWorkshopDirectory(out var workshopDirectory));
        var mods = workshopDirectory
            .EnumerateDirectories(recursive: false)
            .Select(ModLoader.LoadModFromDirectory)
            .ToArray();

        mods.Should().NotBeEmpty();
        if (mods.Length == 1) return;

        var deltaProvider = new JsonDataFileDeltaProvider();

        var left = mods[0];
        for (var i = 1; i < mods.Length; i++)
        {
            var right = mods[i];

            var sameFiles = ModComparer.GetSameFiles(left, right);
            if (sameFiles.Length == 0) continue;

            foreach (var sameFile in sameFiles)
            {
                var leftJsonDataFile = left.JsonDataFiles[sameFile];
                var rightJsonDataFile = right.JsonDataFiles[sameFile];

                var deltas = deltaProvider.Diff(leftJsonDataFile, rightJsonDataFile);
                foreach (var delta in deltas)
                {
                    _testOutputHelper.WriteLine(delta.ToString());
                }
            }
        }
    }
}
