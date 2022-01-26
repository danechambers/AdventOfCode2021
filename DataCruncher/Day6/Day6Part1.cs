namespace DataCruncher.Day6;

public class Day6Part1 : IDataCruncher
{
    private readonly Day6Data day6Data;

    public Day6Part1(Day6Data day6Data)
    {
        this.day6Data = day6Data;
    }

    public async Task<Result> Crunch(CancellationToken? cancelToken = null) =>
        Result.Success(
            (await day6Data.GetDataFromGithub())
                .First()
                .Split(',')
                .GetFish()
                .RunAllCycles(80, cancelToken)
                .CountTheFish());
}

public class Day6Part2 : IDataCruncher
{
    private readonly Day6Data day6Data;

    public Day6Part2(Day6Data day6Data)
    {
        this.day6Data = day6Data;
    }

    public async Task<Result> Crunch(CancellationToken? cancelToken = null) =>
        Result.Success(
            (await day6Data.GetDataFromGithub())
                .First()
                .Split(',')
                .GetFish()
                .RunAllCycles(256, cancelToken)
                .CountTheFish());
}