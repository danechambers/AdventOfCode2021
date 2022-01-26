using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataCruncher;
using DataCruncher.Day6;

IHost host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .ConfigureContainer<ContainerBuilder>((context, builder) =>
    {
        builder.RegisterModule<Day6Module>();
    })
    .Build();

await host.RunAsync();
