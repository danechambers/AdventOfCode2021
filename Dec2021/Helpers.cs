using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace Dec2021
{
    public static class Helpers
    {
        public static IReadOnlyList<string> ReadFileData(string filePath) =>
            ImmutableArray.Create(File.ReadAllLines(filePath));
    }
}