using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using static Dec2021.Helpers;

namespace Dec2021.Day6;

[TestFixture]
public class Day6Tests
{
    [Test(ExpectedResult = 365131)]
    public async Task<int> Part1()
    {
        var fileData = await GetDataUri("Day6/day6input").GetDataAsync();
        var allFish = fileData.First().Split(',').GetFish().ToImmutableList();
        return allFish.RunAllCycles(80).Last().Count;
    }
}