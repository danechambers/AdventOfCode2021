using System.Collections.Immutable;

namespace Dec2021.Day4.Models;

public record GameBoard(IImmutableSet<GameBoardValue> Values, bool BingoNumberFound = false);
