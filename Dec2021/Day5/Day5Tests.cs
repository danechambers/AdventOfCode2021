using System.Threading.Tasks;
using NUnit.Framework;
using static Dec2021.Helpers;

namespace Dec2021.Day5;

[TestFixture]
public class Day5Tests
{
    [Test]
    public async Task Part1()
    {
        var fileData = await GetDataUri("Day5/day5input").GetDataAsync();
    }
}