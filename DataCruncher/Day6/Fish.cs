namespace DataCruncher.Day6;

public readonly record struct Fish(int DaysUntilSpawn);

/// <summary>
/// Intermediary data class for use with aggregating fish counts
/// </summary>
public readonly record struct FishCount(Fish Fish, long Count);