using System.Collections.Immutable;
using System.Numerics;

namespace DataCruncher.Day6;

public readonly record struct Fish2(Guid Id, int DaysUntilSpawn, BigInteger SpawnedChildren);

static class FishCounterMethods
{
    static void CatalogFish(HashSet<Fish2> allfish, Fish2 currentFish)
    {
        allfish.Remove(currentFish);
        var fishAfterCycle = currentFish with { DaysUntilSpawn = currentFish.DaysUntilSpawn - 1 };
        allfish.Add(fishAfterCycle);
    }
}