using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using static Dec2021.Helpers;

namespace Dec2021.Day6;

[TestFixture]
public class Day6Tests
{
    [Test(ExpectedResult = 5934)]
    public async Task<int> Part1()
    {
        var fileData = await ReadFileData("/home/dane/Source/AdventOfCode2021/Dec2021/Day6/day6input");
        var allFish = fileData.First().Split(',').GetFish().ToImmutableList();
        return allFish.RunAllCycles(80);
    }
}