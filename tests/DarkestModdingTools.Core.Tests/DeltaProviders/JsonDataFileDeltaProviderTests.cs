using System.IO;
using System.Linq;
using DarkestModdingTools.Core.DeltaProviders;
using DarkestModdingTools.Core.GameFiles;
using DarkestModdingTools.Core.Parsers;
using FluentAssertions;
using NexusMods.Paths;
using Xunit;

namespace DarkestModdingTools.Core.Tests.DeltaProviders;

public class JsonDataFileDeltaProviderTests
{
    [Theory]
    [InlineData("json/abbey.building.json")]
    [InlineData("json/base.buffs.json")]
    public void Test_Diff_NotDifferent(string file)
    {
        var parser = new JsonDataFileParser();

        JsonDataFile left;
        JsonDataFile right;

        using (var stream = TestHelpers.GetTestFile(file))
        using (var res = parser.ParseFile(stream, "example.json"))
        {
            res.HasException().Should().BeFalse();
            left = res.GetValue();
            right = res.GetValue();
        }

        var diffProvider = new JsonDataFileDeltaProvider();
        var diff = diffProvider.Diff(left, right);
        diff.Should().BeEmpty();
    }

    [Fact]
    public void Test_Diff_Different()
    {
        var parser = new JsonDataFileParser();

        JsonDataFile left;
        JsonDataFile right;

        using (var stream = TestHelpers.GetTestFile("json/abbey.building.json"))
        using (var res = parser.ParseFile(stream, "campaign/town/buildings/abbey/abbey.building.json"))
        {
            res.HasException().Should().BeFalse();
            left = res.GetValue();
        }

        using (var stream = TestHelpers.GetTestFile("json/abbey.building.json-modified"))
        using (var res = parser.ParseFile(stream, "campaign/town/buildings/abbey/abbey.building.json"))
        {
            res.HasException().Should().BeFalse();
            right = res.GetValue();
        }

        var diffProvider = new JsonDataFileDeltaProvider();
        var diff = diffProvider.Diff(left, right);
        diff.Should().ContainSingle();

        var item = diff[0];
        item.Path.Should().Be("$.data.activities[1].data.cost_upgrades[0].cost_currency.amount");
        item.Delta.Current.Value.GetValue<int>().Should().Be(9999);
        item.Delta.Previous.Value.GetValue<int>().Should().Be(1250);
        item.Delta.Kind.Should().Be(DeltaKind.Modified);
    }

    [SkippableFact]
    public void Test_Diff_WithGameFiles()
    {
        Skip.IfNot(TestHelpers.TryGetGameDirectory(out var gameDirectory), "Game path wasn't set");
        var jsonFiles = gameDirectory.EnumerateFiles(new Extension(".json"), recursive: true).ToArray();
        jsonFiles.Should().NotBeEmpty();

        var parser = new JsonDataFileParser();
        var diffProvider = new JsonDataFileDeltaProvider();

        jsonFiles.Should().AllSatisfy(jsonFile =>
        {
            JsonDataFile left;
            JsonDataFile right;

            var gamePath = jsonFile.RelativeTo(gameDirectory);
            using (var stream = jsonFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var res = parser.ParseFile(stream, gamePath))
            {
                res.HasException().Should().BeFalse();
                if (res == ParserResult<JsonDataFile>.Unsupported) return;

                left = res.GetValue();
                right = res.GetValue();
            }

            var diff = diffProvider.Diff(left, right);
            diff.Should().BeEmpty();
        });
    }
}
