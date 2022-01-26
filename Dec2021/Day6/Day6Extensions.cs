using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Dec2021.Day6;

public record struct Fish(int DaysUntilSpawn);

/// <summary>
/// A function that adds updates to the update builder.
/// </summary>
public delegate void FishDayCycleUpdate(ImmutableList<Fish>.Builder updateBuilder);

public delegate IEnumerable<Fish> MaybeSpawnFish();

public static class Day6Extensions
{
    public static IEnumerable<Fish> GetFish(this IEnumerable<string> values) =>
        values.Select(value => new Fish(int.Parse(value)));

    public static IEnumerable<ImmutableList<Fish>> RunAllCycles(this ImmutableList<Fish> allFish, int cycles)
    {
        var cycleUpdateBuilder = allFish.ToBuilder();

        while (cycles-- > 0)
        {
            allFish
                .Select(TrySpawnFish)
                .Aggregate((currentUpdate, nextUpdate) => currentUpdate + nextUpdate)
                .Invoke(cycleUpdateBuilder);

            allFish = cycleUpdateBuilder.ToImmutableList();

            yield return allFish;
        }
    }

    public static IEnumerable<Fish> RunAllCycles2(this IEnumerable<Fish> allFish, int cycles)
    {
        while(cycles-- > 0)
        {
            var fishPartitioner = Partitioner.Create(allFish);
            var fishQuery = fishPartitioner
                .AsParallel()
                .SelectMany(fish => fish.TrySpawnFish2());
            
            var newFishTracker = Enumerable.Empty<Fish>();

            fishQuery.ForAll(fish => 
                ImmutableInterlocked.Update(ref newFishTracker, tracker => tracker.Append(fish)));
            
            allFish = newFishTracker;
        }

        return allFish;
    }

    private static void AddFishToEnumerable(ref IEnumerable<Fish> newFishTracker, Fish fish)
    {
        newFishTracker = newFishTracker.Append(fish);
    }

    public static Fish MoveFishCloserToSpawn(this Fish fish) => fish with { DaysUntilSpawn = fish.DaysUntilSpawn - 1 };
    public static Fish ResetFish() => new(6);
    public static Fish NewBornFish() => new(8);

    public static IEnumerable<Fish> TrySpawnFish2(this Fish fish)
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

    public static FishDayCycleUpdate TrySpawnFish(
        this Fish fish,
        int fishIndex) =>
        fish.DaysUntilSpawn == 0
            ? fish.SpawnFish(fishIndex)
            : fish.SetFishCloserToSpawn(fishIndex);

    private static FishDayCycleUpdate SpawnFish(
        this Fish fish,
        int fishIndex) =>
        fishStateUpdate =>
        {
            fishStateUpdate[fishIndex] = ResetFish();
            fishStateUpdate.Add(NewBornFish());
        };

    private static FishDayCycleUpdate SetFishCloserToSpawn(
        this Fish fish,
        int fishIndex) =>
        fishStateUpdate =>
        {
            fishStateUpdate[fishIndex] = fish.MoveFishCloserToSpawn();
        };
}