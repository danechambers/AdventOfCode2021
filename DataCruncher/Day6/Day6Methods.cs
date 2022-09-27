using System.Collections.Immutable;
using static DataCruncher.Utility.Helpers;

namespace DataCruncher.Day6;

static class Day6Methods
{
    public static IEnumerable<Fish> GetFish(this IEnumerable<string> values) =>
        values.Select(value => new Fish(int.Parse(value)));

    public static IEnumerable<Fish> RunAllCycles(
        this IEnumerable<Fish> allFish,
        int cycles,
        Func<Fish, IEnumerable<Fish>> spawnFish)
    {
        while (cycles-- > 0)
        {
            allFish = allFish.SelectMany(spawnFish);
        }

        return allFish;
    }

    public static async Task<double> CountTheFish(
        this IEnumerable<Fish> allFish,
        CancellationToken? cancelToken = null)
    {
        double fishCount = 0;

        var computeTask =
            cancelToken is CancellationToken realToken
                ? Parallel.ForEachAsync(
                    allFish,
                    realToken,
                    async (_, _) => await new ValueTask<double>(Increment(ref fishCount)))
                : Parallel.ForEachAsync(
                    allFish,
                    async (_, _) => await new ValueTask<double>(Increment(ref fishCount)));

        await computeTask;
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

    public static IEnumerable<FishCount> CountInitialFish(this IEnumerable<Fish> fish) =>
        fish.Select( aFish => new FishCount(aFish, 1));

    public static IEnumerable<FishCount> SpawnNewFish(this IReadOnlyDictionary<Fish, long> fishState) =>
        fishState
            .SelectMany(
                state => state.Key.TrySpawnFish(),
                (state, newFish) => new FishCount(newFish, state.Value));

    public static ImmutableDictionary<Fish, long> AssessStateOfFish(this IEnumerable<FishCount> fishCounts)
    {
        var fishStateBuilder = ImmutableDictionary.CreateBuilder<Fish, long>();
        foreach(var fishCount in fishCounts)
        {
            if(fishStateBuilder.ContainsKey(fishCount.Fish))
                fishStateBuilder[fishCount.Fish] += fishCount.Count;
            else
                fishStateBuilder[fishCount.Fish] = fishCount.Count;
        }
        return fishStateBuilder.ToImmutableDictionary();
    }
}