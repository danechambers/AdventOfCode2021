using System.Collections.Concurrent;
using static DataCruncher.Utility.Helpers;

namespace DataCruncher.Day6;

 static class Day6Methods
{
    public static IEnumerable<Fish> GetFish(this IEnumerable<string> values) =>
        values.Select(value => new Fish(int.Parse(value)));

    public static IEnumerable<Fish> RunAllCycles(
        this IEnumerable<Fish> allFish, 
        int cycles, 
        CancellationToken? cancelToken = null)
    {
        while(cycles-- > 0)
        {
            cancelToken?.ThrowIfCancellationRequested();
            allFish = allFish.SelectMany(TrySpawnFish);
        }

        return allFish;
    }

    public static double CountTheFish(this IEnumerable<Fish> allFish)
    {
        double fishCount = 0;

        var allFishQuery = 
            Partitioner.Create(allFish)
                .AsParallel();

        allFishQuery.ForAll(fish => Increment(ref fishCount));

        return fishCount;
    }

    public static Fish MoveFishCloserToSpawn(this Fish fish) => fish with { DaysUntilSpawn = fish.DaysUntilSpawn - 1 };
    public static Fish ResetFish() => new(6);
    public static Fish NewBornFish() => new(8);

    public static IEnumerable<Fish> TrySpawnFish(this Fish fish)
    {
        if (fish.DaysUntilSpawn > 0)
        {
            yield return fish.MoveFishCloserToSpawn();
        }
        else
        {
            yield return ResetFish();
            yield return NewBornFish();
        }
    }
}