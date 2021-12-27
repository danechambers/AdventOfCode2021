using System.Collections.Immutable;

namespace Dec2021.Day4.Models;

public record WinnerCheckResult(ImmutableArray<GameBoard> GameBoards, BingoWinner? PossibleWinner);
