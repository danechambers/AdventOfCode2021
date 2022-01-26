namespace DataCruncher.Day6;
using static DataCruncher.Utility.Helpers;

public class Day6Data
{
    public Task<IReadOnlyList<string>> GetDataFromGithub() =>
        GetDataUri("Day6/day6input").GetDataAsync();
}
