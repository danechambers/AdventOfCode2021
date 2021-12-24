using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using static Dec2021.Helpers;

namespace Dec2021.Day4
{
    [TestFixture]
    public class Day4Tests
    {
        private const int MAGIC_NUMBER = 5;

        [Test]
        public async Task GameBoardReader_ShouldWork()
        {
            var fileData = await GetDataUri("Day4/day4input").GetDataAsync();

            var gameBoards = fileData.GetGameBoards(MAGIC_NUMBER);

            gameBoards.Count().ShouldBe(100);
        }

        [Test]
        public async Task Part1()
        {
            var fileData = await GetDataUri("Day4/day4input").GetDataAsync();

            var bingoNumbers = fileData.First().Split(',')
                .Select(value => int.Parse(value))
                .Select<int, BoardNumber>(value => new(value, false));

            var gameBoards = fileData.GetGameBoards(MAGIC_NUMBER).ToImmutableArray();

            // var bingoNumber = bingoNumbers.First(num => num.Value == 7);
            // var gameBoard = gameBoards.First();

            // var (boardPosition, boardValue) = gameBoard.BoardNumberIterator().First(board => board.Number.Value == bingoNumber.Value);
            // gameBoard.Update(boardPosition, boardValue with { Found = bingoNumber.Found });

            int winningIndex = -1;

            foreach (var bingoNumber in bingoNumbers)
            {
                var result = Parallel.For(0, gameBoards.Length, (index, state) =>
                {
                    var gameBoard = gameBoards[index];
                    var boardValues =
                        gameBoard.BoardNumberIterator().Where(value => value.Number == bingoNumber);

                    foreach (var boardValue in boardValues)
                    {
                        var (boardPosition, boardNumber) = boardValue;
                        gameBoard.Update(boardPosition, boardNumber with { Found = true });
                    }

                    ImmutableInterlocked.Update(ref gameBoards, boards => boards.SetItem(index, gameBoard));

                    for (var i = 0; i < MAGIC_NUMBER; i++)
                    {
                        var foundXWinner = gameBoard.BoardNumberIterator().Where(boardValue => boardValue.Position.x == index).All(value => value.Number.Found);
                        var foundYWinner = gameBoard.BoardNumberIterator().Where(boardValue => boardValue.Position.y == index).All(value => value.Number.Found);
                        if (foundXWinner || foundYWinner)
                        {
                            state.Break();
                            break;
                        }
                    }
                });

                if (result.LowestBreakIteration is long breakIndex)
                {
                    winningIndex = (int)breakIndex;
                    break;
                }
            }

            winningIndex.ShouldBeGreaterThanOrEqualTo(0);
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

        public static IEnumerable<GameBoardValue> BoardNumberIterator(this BoardNumber[,] gameBoard)
        {
            for (var i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (var j = 0; j < gameBoard.GetLength(1); j++)
                {
                    yield return new(new(i, j), gameBoard[i, j]);
                }
            }
        }

        public static void Update(this BoardNumber[,] gameBoard, BoardPosition position, BoardNumber value) =>
            gameBoard[position.x, position.y] = value;

        public static IEnumerable<BoardNumber[,]> GetGameBoards(this IEnumerable<string> fileData, int magicNumber) =>
            fileData.Skip(1)
                .SelectMany(line => line.Split())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => int.Parse(value.Trim()))
                .Select<int, BoardNumber>(value => new(value))
                .Select((number, index) => (Number: number, NumberIndex: index % magicNumber))
                .BoardGameIterator(magicNumber);
    }

    public record BoardNumber(int Value, bool Found = false);
    public record BoardPosition(int x, int y);
    public record GameBoardValue(BoardPosition Position, BoardNumber Number);
}