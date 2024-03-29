namespace DataCruncher.Day6;

public class Day6Part1 : IDataCruncher
{
    private readonly Day6Data day6Data;

    public Day6Part1(Day6Data day6Data)
    {
        this.day6Data = day6Data;
    }

    public async Task<Result> Crunch(CancellationToken? cancelToken = null)
    {
        var data = await day6Data.GetDataFromGithub();
        var count =
            await data.First()
                .Split(',')
                .GetFish()
                .RunAllCycles(80, fish => fish.TrySpawnFish())
                .CountTheFish(cancelToken);
        return Result.Success(count);
    }
}

public class Day6Part2 : IDataCruncher
{
    private readonly Day6Data day6Data;

    public Day6Part2(Day6Data day6Data)
    {
        this.day6Data = day6Data;
    }

    public async Task<Result> Crunch(CancellationToken? cancelToken = null)
    {
        var data = await day6Data.GetDataFromGithub();
        var count =
            await data.First()
                .Split(',')
                .GetFish()
                .RunAllCycles(256, fish => fish.TrySpawnFish())
                .CountTheFish(cancelToken);

        return Result.Success(count);
    }
}

public class TheBestCruncher : IDataCruncher
{
    private readonly Day6Data day6Data;

    public TheBestCruncher(Day6Data day6Data)
    {
        this.day6Data = day6Data;
    }

    public async Task<Result> Crunch(CancellationToken? cancelToken = null)
    {
        var data = await day6Data.GetDataFromGithub();
        var initialFishState = data.First()
            .Split(',')
            .GetFish()
            .CountInitialFish()
            .AssessStateOfFish();

        var finalFishState = Enumerable.Range(1, 256)
            .Aggregate(
                initialFishState,
                (fishState, _) => fishState.SpawnNewFish().AssessStateOfFish());

        var finalFishCount = finalFishState.Values.Sum();
        return Result.Success(finalFishCount);
    }
}