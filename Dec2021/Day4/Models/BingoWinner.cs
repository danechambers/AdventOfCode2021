namespace Dec2021.Day4.Models;

public record BingoWinner
{
    public BingoWinner(BoardNumber winningNumber, int winningGameBoardIndex)
    {
        WinningNumber = winningNumber;
        WinningGameBoardIndex = winningGameBoardIndex;
    }

    public BoardNumber WinningNumber { get; init; }

    ///<summar>
    /// The index of the winning game board in the data set
    ///</summary>
    public int WinningGameBoardIndex { get; init; }
}
