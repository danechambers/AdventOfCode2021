using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using static Dec2021.Helpers;

namespace Dec2021.Day6;

[TestFixture]
public class Day6Tests
{
    [Ignore("Refactoring...")]
    [TestCase(80, TestName = "Part1", ExpectedResult = 365131)]
    // [TestCase(256, TestName = "Part2", ExpectedResult = 365131)]
    public async Task<int> Part1(int cycles)
    {
        var fileData = await GetDataUri("Day6/day6input").GetDataAsync();
        var allFish = fileData.First().Split(',').GetFish().ToImmutableList();
        return allFish.RunAllCycles(cycles).Last().Count;
    }

    [Ignore("Refactoring...")]
    // [TestCase(80, TestName = "Part1.2", ExpectedResult = 365131)]
    // [TestCase(256, TestName = "Part2", ExpectedResult = 365131)]
    [Test(ExpectedResult = 365131)]
    public async Task<int> Part2()
    {
        var fileData = await GetDataUri("Day6/day6input").GetDataAsync();
        // var fileData = await ReadFileData("/home/dane/Source/AdventOfCode2021/Dec2021/Day6/sampleinput");
        var allFish = fileData.First().Split(',').GetFish().ToImmutableArray();
        var cyclesCompleted = allFish.RunAllCycles2(80);
        return cyclesCompleted.Count();
    }
}