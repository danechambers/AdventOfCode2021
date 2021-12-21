using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Dec2021
{
    public static class Helpers
    {
        public static IReadOnlyList<string> ReadFileData(string filePath) =>
            File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToImmutableArray();
    }
}