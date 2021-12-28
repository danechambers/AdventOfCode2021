using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Dec2021.Day4.Models;
using NUnit.Framework;
using static Dec2021.Helpers;
using static Dec2021.Day4.Day4Extensions;

namespace Dec2021.Day4;

[TestFixture]
public class Day4Tests
{
    private const int BINGO_SQUARE_LENGTH = 5;
    private const string INPUT_FOLDER_FILE = "Day4/day4input";

    private static readonly Lazy<Task<IReadOnlyList<string>>> getDay4Input =
        new Lazy<Task<IReadOnlyList<string>>>(
            () => GetDataUri(INPUT_FOLDER_FILE).GetDataAsync());
    private static Task<IReadOnlyList<string>> Day4Input() => getDay4Input.Value;

    [Test(ExpectedResult = 100)]
    public async Task<int> GameBoardReader_ShouldWork() =>
        (await Day4Input())
            .GetGameBoards(BINGO_SQUARE_LENGTH)
            .Count();

    [Test(ExpectedResult = 45031)]
    public async Task<int> Part1() =>
        (await Day4Input())
            .GetBingoNumbers()
            .FindWinningBoards(
                (await Day4Input())
                    .GetGameBoards(BINGO_SQUARE_LENGTH)
                    .ToImmutableHashSet(),
                BINGO_SQUARE_LENGTH)
            .Select(CalculateAnswer)
            .First();

    [Test(ExpectedResult = 2568)]
    public async Task<int> Part2() =>
       (await Day4Input())
            .GetBingoNumbers()
            .FindWinningBoards(
                (await Day4Input())
                    .GetGameBoards(BINGO_SQUARE_LENGTH)
                    .ToImmutableHashSet(),
                BINGO_SQUARE_LENGTH)
            .Select(CalculateAnswer)
            .Last();
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
            .Select((number, index) => (Number: number, RowIndex: index % bingoSquareLength))
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

    public static ImmutableArray<BingoWinner> FindWinningBoards(
        this IEnumerable<BoardNumber> bingoNumbers,
        IImmutableSet<GameBoard> gameBoards,
        int bingoSquareLength)
    {
        var allWinners = ImmutableList<BingoWinner>.Empty.ToBuilder();

        foreach (var bingoNumber in bingoNumbers)
        {
            var (updatedGameBoards, winners) = bingoNumber.GetWinnners(gameBoards, bingoSquareLength);
            allWinners.AddRange(winners);
            gameBoards = updatedGameBoards.Except(winners.Select(winner => winner.WinningGameBoard));
        }

        return allWinners.ToImmutableArray();
    }

    public static (IImmutableSet<GameBoard> GameBoards, IEnumerable<BingoWinner> Winners) GetWinnners(
        this BoardNumber bingoNumber,
        IImmutableSet<GameBoard> gameBoards,
        int bingoSquareLength)
    {
        var winnersFoundFromUpdate = new ConcurrentQueue<BingoWinner>();

        Parallel.ForEach(gameBoards, gameBoard =>
        {
            var markedGameBoard =
                gameBoard.Values
                    .Where(boardValue => boardValue.Number == bingoNumber)
                    .MarkNumbersFoundOnBoard(gameBoard);

            ImmutableInterlocked.Update(ref gameBoards,
                board => board.Remove(gameBoard).Add(markedGameBoard));

            if (markedGameBoard.CheckForWinner(bingoSquareLength))
                winnersFoundFromUpdate.Enqueue(new(bingoNumber, markedGameBoard));
        });

        return (gameBoards, winnersFoundFromUpdate);
    }

    public static int CalculateAnswer(BingoWinner winner)
    {
        var (winningNumber, winningGameBoard) = winner;

        var unmarkedSum =
            winningGameBoard
                .Values
                .Where(value => !value.Number.Marked)
                .Sum(value => value.Number.Value);

        return winningNumber.Value * unmarkedSum;
    }
}