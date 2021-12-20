using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using static Dec2021.Day3.EnumerableExtensions;

namespace Dec2021.Day3
{
    [TestFixture]
    public class Day3Tests
    {
        [Test]
        public void Part1()
        {
            var answer =
                File.ReadAllLines("/home/dane/Source/AdventOfCode2021/Dec2021/Day3/day3input")
                    .CreateDataTableFromFile()
                    .SelectBits()
                    .MapBitsToTablePosition()
                    .FillDataTable()
                    .GetBitCounts()
                    .GetGammaBits()
                    .GetGammaAndEpsilon()
                    .AggregateGammaAndEpsilon();

            TestContext.WriteLine($"The answer is {answer}");
            answer.ShouldBe(1025636);
        }

        [Test]
        public async Task Part2()
        {
            var fileDataStream = File.ReadAllLines("/home/dane/Source/AdventOfCode2021/Dec2021/Day3/day3input");
            var data =
                    fileDataStream
                    .CreateDataTableFromFile()
                    .SelectBits()
                    .MapBitsToTablePosition()
                    .FillDataTable();

            var oxygenGeneratorRating = data.GetLifeSupportRatingWithBitCriteria(OxygenGeneratorBitCriteria, fileDataStream);
            var c02ScrubberRating = data.GetLifeSupportRatingWithBitCriteria(C02ScrubberBitCriteria, fileDataStream);
            var testData = await Task.WhenAll(oxygenGeneratorRating, c02ScrubberRating);

            testData.ShouldSatisfyAllConditions(
                () => oxygenGeneratorRating.Result.ShouldBe(3089),
                () => c02ScrubberRating.Result.ShouldBe(257));

            TestContext.WriteLine($"The answer is {oxygenGeneratorRating.Result * c02ScrubberRating.Result}");
        }

        [TestCase(7,3, ExpectedResult = '1')]
        [TestCase(3,7, ExpectedResult = '0')]
        [TestCase(3,3, ExpectedResult = '1')]
        public char OxygenSanityCheck(int count1s, int count0s) =>
            OxygenGeneratorBitCriteria(count1s, count0s);


        [TestCase(7,3, ExpectedResult = '0')]
        [TestCase(3,7, ExpectedResult = '1')]
        [TestCase(3,3, ExpectedResult = '0')]
        public char C02SanityCheck(int count1s, int count0s) =>
            C02ScrubberBitCriteria(count1s, count0s);
    }

    public static class EnumerableExtensions
    {
        public static Func<string, int> ConvertStringToInt => value => Convert.ToInt32(value, 2);

        public static (IEnumerable<(int ColumnIndex, int RowIndex, char Value)> DataMeta, char[][] DataTable) MapBitsToTablePosition(
            this (IEnumerable<char> BitStream, char[][] DataTable) data ) =>
            (data.BitStream.BitIterator(data.DataTable.Length), data.DataTable);

        public static IEnumerable<(int ColumnIndex, int RowIndex, char Value)> BitIterator(this IEnumerable<char> bits, int columnReset)
        {
            int columnIndex = 0,
                rowIndex = 0;

            foreach(var bit in bits)
            {
                yield return (columnIndex, rowIndex, bit);

                if(++columnIndex == columnReset)
                {
                    columnIndex = 0;
                    rowIndex++;
                }
            }
        }

        public static char[][] FillDataTable(
            this (IEnumerable<(int ColumnIndex, int RowIndex, char Value)> DataMeta, char[][] DataTable) data ) =>
                data.DataMeta.Aggregate(data.DataTable, (table, meta) => 
                {
                    table[meta.ColumnIndex][meta.RowIndex] = meta.Value;
                    return table;
                });

        public static (IEnumerable<char> BitStream, char[][] DataTable) SelectBits(
            this (string[] DataStream, char[][] DataTable) data) =>
            (data.DataStream.SelectMany(bits => bits.ToCharArray()), data.DataTable);

        public static IEnumerable<(int Count1s, int Count0s)> GetBitCounts(this char[][] data) =>
            data.Select(column => (column.GetCountOf1s(), column.GetCountOf0s()));

        public static int GetCountOf1s(this IEnumerable<char> bits) => bits.Count(value => value == '1');
        public static int GetCountOf0s(this IEnumerable<char> bits) => bits.Count(value => value == '0');

        public static (string[] DataStream, char[][] DataTable) CreateDataTableFromFile(this string[] fileData)
            => (fileData, fileData.First().ToCharArray().Select(_ => new char[fileData.Length]).ToArray());
    
        public static string GetStringFromChars(this IEnumerable<char> chars) => new string(chars.ToArray());

        public static char GetGamma(int count1s, int count0s) => count1s > count0s ? '1' : '0';

        public static IEnumerable<char> GetGammaBits(this IEnumerable<(int Count1s, int Count0s)> data) =>
            data.Select(counts => GetGamma(counts.Count1s, counts.Count0s));

        public static string GetEpsilonFromGamma(string gamma) =>
            gamma.ToCharArray().Select(bit => bit == '0' ? '1' : '0').GetStringFromChars();

        public static IEnumerable<(int Gamma, int Epsilon)> GetGammaAndEpsilon(this IEnumerable<char> gammaBits)
        {
            var gamma = gammaBits.GetStringFromChars();
            var epsilon = GetEpsilonFromGamma(gamma);
            yield return (ConvertStringToInt(gamma), ConvertStringToInt(epsilon));
        }

        public static int AggregateGammaAndEpsilon(this IEnumerable<(int Gamma, int Epsilon)> data) =>
            data.Select(values => values.Gamma * values.Epsilon).Single();

        public static char OxygenGeneratorBitCriteria(int count1s, int count0s) =>
            count1s >= count0s ? '1' : '0';

        public static char C02ScrubberBitCriteria(int count1s, int count0s) =>
            count0s <= count1s ? '0' : '1';

        public static Task<int> GetLifeSupportRatingWithBitCriteria(
            this char[][] dataTable,
            Func<int, int, char> getBitCriteria,
            string[] fileData)
        {
            var dataRowIndexes = new HashSet<int>(Enumerable.Range(0, dataTable[0].Length));

            foreach(var tableColumn in dataTable)
            {
                if(dataRowIndexes.Count == 1)
                    break;

                var count0s = dataRowIndexes.Select(rowIndex => tableColumn[rowIndex]).GetCountOf0s();
                var count1s = dataRowIndexes.Select(rowIndex => tableColumn[rowIndex]).GetCountOf1s();
                var critera = getBitCriteria(count1s, count0s);

                dataRowIndexes.IntersectWith(dataRowIndexes.Where(rowIndex => tableColumn[rowIndex] == critera));
            }

            return Task.FromResult(ConvertStringToInt(fileData[dataRowIndexes.Single()]));
        }
    }
}