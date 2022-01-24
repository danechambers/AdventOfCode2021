using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Dec2021.Day6;

public record struct Fish(int DaysUntilSpawn);

/// <summary>
/// A function that adds updates to the update builder.
/// </summary>
public delegate void FishDayCycleUpdate(ImmutableList<Fish>.Builder updateBuilder);

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

    private static FishDayCycleUpdate TrySpawnFish(
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
            fishStateUpdate[fishIndex] = fish with { DaysUntilSpawn = 6 };
            fishStateUpdate.Add(fish with { DaysUntilSpawn = 8 });
        };

    private static FishDayCycleUpdate SetFishCloserToSpawn(
        this Fish fish,
        int fishIndex) =>
        fishStateUpdate =>
        {
            fishStateUpdate[fishIndex] = fish with { DaysUntilSpawn = fish.DaysUntilSpawn - 1 };
        };
}