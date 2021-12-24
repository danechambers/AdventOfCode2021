using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using static Dec2021.Helpers;

namespace Dec2021.Day4
{
    [TestFixture]
    public class Day4Tests
    {
        [Test]
        public async Task Part1()
        {
            var fileData = await GetDataUri("Day4/day4input").GetDataAsync();

            var bingoNumbers = fileData.First().Split(',')
                .Select(value => int.Parse(value))
                .Select<int, BoardNumber>(value => new(value, true))
                .ToImmutableHashSet();

            const int magicNumber = 5;

            var gameBoards =
                fileData.Skip(1)
                    .SelectMany(line => line.Split())
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Select(value => int.Parse(value.Trim()))
                    .Select<int, BoardNumber>(value => new(value))
                    .Select((number, index) => (Number: number, NumberIndex: index % magicNumber))
                    .BoardGameIterator(magicNumber)
                    .ToImmutableArray();

            gameBoards.Length.ShouldBe(100);
        }
    }

    public static class Day4Extensions
    {
        public static IEnumerable<BoardNumber[,]> BoardGameIterator(
            this IEnumerable<(BoardNumber Number, int NumberIndex)> boardData,
            int magicNumber)
        {
            var gameBoard = new BoardNumber[magicNumber, magicNumber];
            var index = 0;

            foreach (var boardNumber in boardData)
            {
                var (number, numberIndex) = boardNumber;
                gameBoard[index, numberIndex] = number;

                if (numberIndex == magicNumber - 1)
                    ++index;

                if (index == magicNumber)
                {
                    yield return gameBoard;
                    index = 0;
                    gameBoard = new BoardNumber[magicNumber, magicNumber];
                }
            }
        }
    }

    public record BoardNumber(int Value, bool Found = false);
    // public record BoardPosition(int x, int y);
}