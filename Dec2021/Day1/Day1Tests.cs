using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace Dec01;

public class Day1Tests
{
    [Test]
    public void Test1()
    {
        var countOfIncreases = 0;

        var comparisonQueue = new Queue<int>(4);
        var fileData = File.ReadAllLines("/home/dane/Source/AdventOfCode-Dec2021/Dec01/Day1/input");
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