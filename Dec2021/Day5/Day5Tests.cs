using System.Linq;
using System.Threading.Tasks;
using Dec2021.Day5.Records;
using NUnit.Framework;
using Shouldly;
using static Dec2021.Day5.Day5Extensions;
using static Dec2021.Helpers;

namespace Dec2021.Day5;

[TestFixture]
public class Day5Tests
{
    [Test(ExpectedResult = 4655)]
    public async Task<int> Part1() =>
        (await GetDataUri("Day5/day5input").GetDataAsync())
            .ReadLinePoints(
                line => line.SplitLineOnArrow(),
                lineData => lineData.CreateLineEndPoints(
                    dataPoint => dataPoint.CreateLinePoint()))
            .Where(
                AnyConsideration(ConsiderLinesOnXOrYAxis))
            .Select(endPoints => endPoints.CreateLineSegment())
            .SelectMany(segment => segment.Points)
            .GroupBy(segmentPoint => segmentPoint)
            .Count(pointGroup => pointGroup.Count() > 1);

    [Test(ExpectedResult = 20500)]
    public async Task<int> Part2() =>
        (await GetDataUri("Day5/day5input").GetDataAsync())
            .ReadLinePoints(
                line => line.SplitLineOnArrow(),
                lineData => lineData.CreateLineEndPoints(
                    dataPoint => dataPoint.CreateLinePoint()))
            .Where(
                AnyConsideration(
                    ConsiderLinesOnXOrYAxis,
                    Consider45DegreeAxis))
            .Select(endPoints => endPoints.CreateLineSegment())
            .SelectMany(segment => segment.Points)
            .GroupBy(segmentPoint => segmentPoint)
            .Count(pointGroup => pointGroup.Count() > 1);
}

[TestFixture]
public class Day5ExtensionTests
{
    [TestCase("976,35 -> 24,987", "976,35", "24,987")]
    public void StringSplit_ShouldWork(string test, string firstExpected, string secondExpected)
    {
        var result = test.SplitLineOnArrow();

        result.ShouldSatisfyAllConditions(
            () => result[0].ShouldBe(firstExpected),
            () => result[1].ShouldBe(secondExpected)
        );
    }

    [TestCase("1,1 -> 1,3", "1,1|1,2|1,3")]
    [TestCase("1,3 -> 1,1", "1,1|1,2|1,3")]
    [TestCase("9,7 -> 7,7", "9,7|8,7|7,7")]
    [TestCase("7,7 -> 9,7", "9,7|8,7|7,7")]
    [TestCase("1,1 -> 1,1", "")]
    [TestCase("1,1 -> 3,3", "1,1|2,2|3,3")]
    [TestCase("3,3 -> 1,1", "1,1|2,2|3,3")]
    [TestCase("8,0 -> 0,8", "8,0|7,1|6,2|5,3|4,4|3,5|2,6|1,7|0,8")]
    [TestCase("7,9 -> 9,7", "9,7|8,8|7,9")]
    [TestCase("6,4 -> 2,0", "6,4|5,3|4,2|3,1|2,0")]
    [TestCase("5,5 -> 8,2", "5,5|6,4|7,3|8,2")]
    public void GetPointsBetweenSegments_ShouldWork(string test, string expected)
    {
        var expectedSegments = string.IsNullOrEmpty(expected)
            ? Enumerable.Empty<Point>()
            : expected.Split('|')
                .Select(data => data.CreateLinePoint());

        var result =
            test
                .SplitLineOnArrow()
                .CreateLineEndPoints(dataPoint => dataPoint.CreateLinePoint())
                .CreateLineSegment()
                .Points;

        expectedSegments.ToHashSet().SetEquals(result).ShouldBeTrue();
    }

    [TestCase("1,1 -> 1,3", ExpectedResult = false)]
    [TestCase("1,3 -> 1,1", ExpectedResult = false)]
    [TestCase("1,1 -> 3,3", ExpectedResult = true)]
    [TestCase("3,3 -> 1,1", ExpectedResult = true)]
    [TestCase("9,7 -> 7,9", ExpectedResult = true)]
    [TestCase("1,1 -> 1,1", ExpectedResult = false)]
    [TestCase("6,4 -> 2,0", ExpectedResult = true)]
    [TestCase("5,5 -> 8,2", ExpectedResult = true)]
    public bool Consider45DegreeAxis_ShouldWork(string test) =>
        Consider45DegreeAxis(test
            .SplitLineOnArrow()
            .CreateLineEndPoints(dataPoint =>
                dataPoint.CreateLinePoint()));
}