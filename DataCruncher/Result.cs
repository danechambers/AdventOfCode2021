namespace DataCruncher;

public class Result
{
    private readonly string info;

    private Result(string info)
    {
        this.info = info;
    }

    public override string ToString() => info;

    public static Result Success(object answer) =>
        new Result(answer.ToString()
            ?? throw new InvalidOperationException("Answer must have a valid string representation"));

    public static Result Failure(string problem) => new Result(problem);
}
