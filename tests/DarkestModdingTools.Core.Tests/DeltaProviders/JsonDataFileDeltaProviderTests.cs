using DarkestModdingTools.Core.DeltaProviders;
using DarkestModdingTools.Core.GameFiles;
using DarkestModdingTools.Core.Parsers;
using FluentAssertions;
using Xunit;

namespace DarkestModdingTools.Core.Tests.DeltaProviders;

public class JsonDataFileDeltaProviderTests
{
    [Fact]
    public void Test_Diff_NotDifferent()
    {
        var parser = new JsonDataFileParser();

        JsonDataFile left;
        JsonDataFile right;

        using (var stream = TestHelpers.GetTestFile("json/abbey.building.json"))
        using (var res = parser.ParseFile(stream, "campaign/town/buildings/abbey/abbey.building.json"))
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
}
