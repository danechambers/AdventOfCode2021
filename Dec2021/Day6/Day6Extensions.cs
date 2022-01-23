using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Dec2021.Day6;

public record struct Fish(int DaysUntilSpawn);

public static class Day6Extensions
{
    public static IEnumerable<Fish> GetFish(this IEnumerable<string> values) =>
        values.Select(value => new Fish(int.Parse(value)));

    public static int RunAllCycles(this ImmutableList<Fish> allFish, int cycles)
    {
        var fishUpdateBuilder = allFish.ToBuilder();

        while (cycles-- > 0)
        {
            foreach(var update in allFish.Select(TrySpawnFish))
            {
                fishUpdateBuilder = update(fishUpdateBuilder);
            }

            allFish = fishUpdateBuilder.ToImmutableList();
        }

        return allFish.Count;
    }

    public static Func<ImmutableList<Fish>.Builder, ImmutableList<Fish>.Builder> TrySpawnFish(
        this Fish fish,
        int fishIndex) =>
        fish.DaysUntilSpawn == 0
            ? fish.SpawnFish(fishIndex)
            : fish.SetFishCloserToSpawn(fishIndex);

    public static Func<ImmutableList<Fish>.Builder, ImmutableList<Fish>.Builder> SpawnFish(
        this Fish fish,
         int fishIndex) =>
        fishStateUpdate =>
        {
            fishStateUpdate[fishIndex] = fish with { DaysUntilSpawn = 6 };
            fishStateUpdate.Add(fish with { DaysUntilSpawn = 8 });
            return fishStateUpdate;
        };

    public static Func<ImmutableList<Fish>.Builder, ImmutableList<Fish>.Builder> SetFishCloserToSpawn(
        this Fish fish,
        int fishIndex) =>
        fishStateUpdate =>
        {
            fishStateUpdate[fishIndex] = fish with { DaysUntilSpawn = fish.DaysUntilSpawn - 1 };
            return fishStateUpdate;
        };
}