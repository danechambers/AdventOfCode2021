namespace Dec2021.Day4.Models;

public record BingoWinner
{
    public BingoWinner(BoardNumber winningNumber, GameBoard winningBoard)
    {
        WinningNumber = winningNumber;
        WinningBoard = winningBoard;
    }

    public BoardNumber WinningNumber { get; init; }
    public GameBoard WinningBoard { get; init; }
}
