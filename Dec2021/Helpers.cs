using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dec2021
{
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
    }
}