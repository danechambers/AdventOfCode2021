using Autofac;

namespace DataCruncher.Day6;

public class Day6Module : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<Day6Data>().SingleInstance();
        builder
            .RegisterType<Day6Part1>()
            .As<IDataCruncher>()
            .SingleInstance();

        builder
            .RegisterType<TheBestCruncher>()
            .As<IDataCruncher>()
            .SingleInstance();
    }
}