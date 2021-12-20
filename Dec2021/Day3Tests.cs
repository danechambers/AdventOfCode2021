using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace Dec2021
{
    [TestFixture]
    public class Day3Tests
    {
        [Test]
        public void Test1()
        {
            var answer = 
                File.ReadAllLines("/home/dane/Source/AdventOfCode/Dec2021/day3input")
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
            var bitEnumerator = bits.GetEnumerator();

            while(bitEnumerator.MoveNext())
            {
                yield return (columnIndex, rowIndex, bitEnumerator.Current);

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
            data.Select(column =>
                (column.Where(value => value == '1').Count(), column.Where(value => value == '0').Count()));
    
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
    }
}