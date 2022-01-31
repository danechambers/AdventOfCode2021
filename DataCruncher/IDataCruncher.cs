namespace DataCruncher;

public interface IDataCruncher
{
    Task<Result> Crunch(CancellationToken? cancelToken = null);
}