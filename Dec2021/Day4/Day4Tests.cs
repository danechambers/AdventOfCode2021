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
        public async Task FindBingoNumerInGameBoard_ShouldWork()
        {
            var fileData = await GetDataUri("Day4/day4input").GetDataAsync();
            var gameBoard = fileData.GetGameBoards(MAGIC_NUMBER).First();

            GameBoardValue testValue = new(new(2, 2), new(58, false));

            gameBoard
                .GetBingoNumbersFromGameBoard(testValue.Number)
                .Single()
                .ShouldBeEquivalentTo(testValue);
        }

        [Test]
        public async Task Part1()
        {
            var fileData = await GetDataUri("Day4/day4input").GetDataAsync();
            var bingoNumbers = fileData.GetBingoNumbers();
            var gameBoards = fileData.GetGameBoards(MAGIC_NUMBER).ToImmutableArray();

            int? winningIndex = null;
            BoardNumber? winningNumber = null;

            foreach (var bingoNumber in bingoNumbers)
            {
                var result = Parallel.ForEach(gameBoards, (gameBoard, state, index) =>
                {
                    var hasUpdate = false;
                    foreach (var boardValue in gameBoard.GetBingoNumbersFromGameBoard(bingoNumber))
                    {
                        if (!hasUpdate)
                            hasUpdate = true;

                        var (boardPosition, boardNumber) = boardValue;
                        gameBoard.Update(boardPosition, boardNumber with { Found = true });
                    }

                    if (hasUpdate)
                    {
                        ImmutableInterlocked.Update(ref gameBoards, boards => boards.SetItem((int)index, gameBoard));

                        for (var i = 0; i < MAGIC_NUMBER; i++)
                        {
                            var foundXWinner = gameBoard.BoardNumberIterator().Where(boardValue => boardValue.Position.x == i).All(value => value.Number.Found);
                            var foundYWinner = gameBoard.BoardNumberIterator().Where(boardValue => boardValue.Position.y == i).All(value => value.Number.Found);
                            if (foundXWinner || foundYWinner)
                            {
                                state.Break();
                                break;
                            }
                        }
                    }
                });

                if (result.LowestBreakIteration is long breakIndex)
                {
                    winningIndex = (int)breakIndex;
                    winningNumber = bingoNumber;
                    break;
                }
            }

            if(winningIndex is null || winningNumber is null)
            {
                Assert.Fail("No winner found");
                return;
            }

            var unmarkedSum =
                gameBoards[winningIndex.Value].BoardNumberIterator()
                    .Where(value => !value.Number.Found)
                    .Sum(value => value.Number.Value);

            var answer = unmarkedSum * winningNumber.Value;
            TestContext.WriteLine($"The answer is {answer}");
            answer.ShouldBe(45031);
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

        public static IEnumerable<GameBoardValue> GetBingoNumbersFromGameBoard(
            this BoardNumber[,] gameBoard,
             BoardNumber number) =>
            gameBoard.BoardNumberIterator().Where(value => value.Number == number);

        public static IEnumerable<BoardNumber> GetBingoNumbers(this IEnumerable<string> fileData) =>
            fileData.First().Split(',')
                .Select(value => int.Parse(value))
                .Select<int, BoardNumber>(value => new(value, false));
    }

    public record BoardNumber(int Value, bool Found = false);
    public record BoardPosition(int x, int y);
    public record GameBoardValue(BoardPosition Position, BoardNumber Number);
}