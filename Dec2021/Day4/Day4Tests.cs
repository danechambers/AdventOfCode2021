using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Dec2021.Day4.Models;
using NUnit.Framework;
using Shouldly;
using static Dec2021.Helpers;
using static Dec2021.Day4.Day4Extensions;

namespace Dec2021.Day4;

[TestFixture]
public class Day4Tests
{
    private const int BINGO_SQUARE_LENGTH = 5;

    [Test]
    public async Task GameBoardReader_ShouldWork()
    {
        var fileData = await GetDataUri("Day4/day4input").GetDataAsync();

        var gameBoards = fileData.GetGameBoards(BINGO_SQUARE_LENGTH);

        gameBoards.Count().ShouldBe(100);
    }

    [Test]
    public async Task Part1()
    {
        var fileData = await GetDataUri("Day4/day4input").GetDataAsync();
        var gameBoards = fileData.GetGameBoards(BINGO_SQUARE_LENGTH).ToImmutableArray();

        var answer =
            fileData
                .GetBingoNumbers()
                .FindWinningBoard(gameBoards, BINGO_SQUARE_LENGTH)
                .SelectMany(result => result.Winners, (result, winner) => (GameBoards: result.GameBoards, Winner: winner))
                .Select(result => CalculateAnswer(result.GameBoards, result.Winner))
                .First();

        TestContext.WriteLine($"The answer is {answer}");
        answer.ShouldBe(45031);
    }

    [Test]
    public async Task Part2()
    {
        var fileData = await GetDataUri("Day4/day4input").GetDataAsync();
        var gameBoards = fileData.GetGameBoards(BINGO_SQUARE_LENGTH).ToImmutableArray();

        var answer =
            fileData.GetBingoNumbers()
                .FindWinningBoard(gameBoards, BINGO_SQUARE_LENGTH)
                .SelectMany(result => result.Winners, (result, winner) => (GameBoards: result.GameBoards, Winner: winner))
                .Select(result => CalculateAnswer(result.GameBoards, result.Winner))
                .Last();

        TestContext.WriteLine($"The answer is {answer}");
        answer.ShouldBe(2568);
    }
}

public static class Day4Extensions
{
    public static IEnumerable<GameBoard> BoardGameIterator(
        this IEnumerable<(BoardNumber Number, int RowIndex)> boardData,
        int bingoSquareLength)
    {
        var bingoSquareArea = bingoSquareLength * bingoSquareLength;
        var gameBoardValues = new HashSet<GameBoardValue>(bingoSquareArea);
        var columnIndex = 0;

        foreach (var boardNumber in boardData)
        {
            var (number, rowIndex) = boardNumber;
            BoardPosition position = new(rowIndex, columnIndex);
            gameBoardValues.Add(new(position, number));

            if (rowIndex == bingoSquareLength - 1)
                ++columnIndex;

            if (columnIndex == bingoSquareLength)
            {
                yield return new(gameBoardValues.ToImmutableHashSet());
                columnIndex = 0;
                gameBoardValues = new HashSet<GameBoardValue>(bingoSquareArea);
            }
        }
    }

    public static IEnumerable<GameBoard> GetGameBoards(this IEnumerable<string> fileData, int bingoSquareLength) =>
        fileData.Skip(1)
            .SelectMany(line => line.Split())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => int.Parse(value.Trim()))
            .Select<int, BoardNumber>(value => new(value))
            .Select((number, index) => (Number: number, ColumnIndex: index % bingoSquareLength))
            .BoardGameIterator(bingoSquareLength);

    public static IEnumerable<BoardNumber> GetBingoNumbers(this IEnumerable<string> fileData) =>
        fileData.First().Split(',')
            .Select(value => int.Parse(value))
            .Select<int, BoardNumber>(value => new(value));

    public static GameBoard MarkNumbersFoundOnBoard(this IEnumerable<GameBoardValue> values, GameBoard gameBoard) =>
        values.Aggregate(gameBoard, (currentBoard, bingoNumber) =>
            {
                var newBoardValues =
                    currentBoard.Values
                        .Remove(bingoNumber)
                        .Add(bingoNumber with { Number = bingoNumber.Number with { Marked = true } });

                return currentBoard with { Values = newBoardValues, BingoNumberFound = true };
            });

    public static GameBoard UpdateGameBoard(
        this GameBoard data,
        ref ImmutableArray<GameBoard> gameBoardData,
        int index)
    {
        if (data.BingoNumberFound)
            ImmutableInterlocked.Update(
                ref gameBoardData,
                // reset the updated flag back to false in order to ensure that we don't accidentially
                // think a bingo number was found on the board on the next loop
                boards => boards.SetItem(index, data with { BingoNumberFound = false }));

        return data;
    }

    public static bool AllBoardNumbersAreMarked(
        this IEnumerable<GameBoardValue> gameBoard,
        Func<BoardPosition, bool> isTargetPosition) =>
        gameBoard
            .Where(boardValue => isTargetPosition(boardValue.Position))
            .All(boardValue => boardValue.Number.Marked);

    public static bool CheckForWinner(this GameBoard data, int bingoSquareLength)
    {
        if (!data.BingoNumberFound)
            return false;

        var bingoSquareIndexIterator = Enumerable.Range(0, bingoSquareLength);

        var xDimensionWinnerCheck =
            bingoSquareIndexIterator.Select(index =>
                data.Values.AllBoardNumbersAreMarked(position => position.x == index));
        var yDimensionWinnerCheck =
            bingoSquareIndexIterator.Select(index =>
                data.Values.AllBoardNumbersAreMarked(position => position.y == index));

        var isWinner =
            xDimensionWinnerCheck
                .Concat(yDimensionWinnerCheck)
                .FirstOrDefault(foundWinner => foundWinner);

        return isWinner;
    }

    public static IEnumerable<WinnerCheckResult> FindWinningBoard(
        this IEnumerable<BoardNumber> bingoNumbers,
        ImmutableArray<GameBoard> gameBoards,
        int bingoSquareLength)
    {
        var winnerGameBoardIndexTracker = ImmutableHashSet<int>.Empty;

        foreach (var bingoNumber in bingoNumbers)
        {
            var result = bingoNumber.TryGetWinner(
                gameBoards,
                bingoSquareLength,
                winnerGameBoardIndexTracker);

            yield return result;

            winnerGameBoardIndexTracker =
                winnerGameBoardIndexTracker
                    .Union(result.Winners.Select(winner => winner.WinningGameBoardIndex));

            gameBoards = result.GameBoards;
        }
    }

    public static WinnerCheckResult TryGetWinner(
        this BoardNumber bingoNumber,
        ImmutableArray<GameBoard> gameBoards,
        int bingoSquareLength,
        IReadOnlySet<int> previousWinners)
    {
        var winnersFoundFromUpdate = new ConcurrentQueue<BingoWinner>();

        Parallel.ForEach(gameBoards, (gameBoard, _, index) =>
        {
            int gameBoardIndex = (int)index;
            if (previousWinners.Contains(gameBoardIndex))
                return;

            var foundWinner =
                gameBoard.Values
                    .Where(boardValue => boardValue.Number == bingoNumber)
                    .MarkNumbersFoundOnBoard(gameBoard)
                    .UpdateGameBoard(ref gameBoards, gameBoardIndex)
                    .CheckForWinner(bingoSquareLength);

            if (foundWinner)
                winnersFoundFromUpdate.Enqueue(new(bingoNumber, gameBoardIndex));
        });

        if (winnersFoundFromUpdate.Count > 1)
            TestContext.WriteLine($"{bingoNumber.Value} - Found more than {winnersFoundFromUpdate.Count} winners");

        return new(gameBoards, winnersFoundFromUpdate.ToImmutableHashSet());
    }

    public static int CalculateAnswer(IReadOnlyList<GameBoard> gameBoards, BingoWinner winner)
    {
        var winningNumber = winner.WinningNumber.Value;
        var unmarkedSum =
            gameBoards[winner.WinningGameBoardIndex]
                .Values
                .Where(value => !value.Number.Marked)
                .Sum(value => value.Number.Value);

        return winningNumber * unmarkedSum;
    }
}