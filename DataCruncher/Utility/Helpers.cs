using System.Collections.Immutable;

namespace DataCruncher.Utility;
public static class Helpers
{
    public static async Task<IEnumerable<string>> ReadFileData(string filePath) =>
        (await File.ReadAllLinesAsync(filePath))
            .Where(line => !string.IsNullOrWhiteSpace(line));

    private const string RootGitRepoDec2021Folder =
        "https://raw.githubusercontent.com/danechambers/AdventOfCode2021/master/Dec2021";

    public static Uri GetDataUri(string dataFolder) =>
        new Uri(Path.Join(RootGitRepoDec2021Folder, dataFolder));

    private static HttpClient Client => new HttpClient();

    public static async Task<IReadOnlyList<string>> GetDataAsync(this Uri uri)
    {
        using var dataStream = await Client.GetStreamAsync(uri);
        return CreateDataIterator(dataStream).ToImmutableArray();
    }

    private static IEnumerable<string> CreateDataIterator(Stream dataStream)
    {
        using var dataStreamReader = new StreamReader(dataStream);

        string? line;
        while ((line = dataStreamReader.ReadLine()) is not null)
        {
            if (!string.IsNullOrWhiteSpace(line))
                yield return line;
        }
    }

    public static double Add(ref double location1, double value)
    {
        double newCurrentValue = location1; // non-volatile read, so may be stale
        while (true)
        {
            double currentValue = newCurrentValue;
            double newValue = currentValue + value;
            newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);
            if (newCurrentValue == currentValue)
                return newValue;
        }
    }

    public static double Increment(ref double location1) => Add(ref location1, 1);
}