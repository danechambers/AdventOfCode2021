using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using static Dec2021.Helpers;

namespace Dec01;

public class Day1Tests
{
    [Test]
    public async Task Part1()
    {
        var countOfIncreases = 0;
        var fileData = await GetDataUri("Day1/day1input").GetDataAsync();

        fileData
            .Select(row =>
            {
                var value = int.Parse(row);
                return value;
            })
            .Aggregate((current, next) =>
            {
                countOfIncreases += Day1Increment(current, next);
                return next;
            });

        TestContext.WriteLine($"Number of increases is {countOfIncreases}");
        countOfIncreases.ShouldBe(1832);
    }

    [Test]
    public async Task Part2()
    {
        var countOfIncreases = 0;

        var comparisonQueue = new Queue<int>(4);
        var fileData = await GetDataUri("Day1/day1input").GetDataAsync();

        fileData
            .Select(row =>
            {
                var value = int.Parse(row);
                comparisonQueue.Enqueue(value);
                return value;
            })
            .Aggregate((current, next) =>
            {
                countOfIncreases += Day1P2Increment(comparisonQueue);
                return next;
            });

        TestContext.WriteLine($"Number of increases is {countOfIncreases}");
        countOfIncreases.ShouldBe(1858);
    }

    private static int Day1Increment(int current, int next) => next > current ? 1 : 0;

    private static int Day1P2Increment(Queue<int> queue)
    {
        if (queue.Count == 4)
        {
            var firstSlice = queue.Take(3).Sum();
            var secondSlice = queue.Skip(1).Take(3).Sum();
            queue.Dequeue();
            if (secondSlice > firstSlice)
                return 1;
        }

        return 0;
    }
}