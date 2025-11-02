// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using DataExtraction.Application.Interfaces;
using DataExtraction.Application.Services;
using DataExtraction.Infrastructure.Repositories;

if (args.Length < 2)
{
    Console.WriteLine("Invalid Input. Reference : <BankName> <InputFile>");
    return;
}

var bankName = args[0];
var inputFile = args[1];

var services = new ServiceCollection();
RegisterServices(services);

var provider = services.BuildServiceProvider();
var service = provider.GetRequiredService<IDataExtractionService>();

//Start the execution
await service.ExecuteAsync(bankName, inputFile);


//Register all services and parsers
void RegisterServices(ServiceCollection servicesCollection)
{
    var adapterAssembly = Assembly.Load("DataExtraction.Infrastructure");

    var parserInterface = typeof(IBankParser);
    var parserTypes = adapterAssembly
        .GetTypes()
        .Where(t => parserInterface.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

    foreach (var type in parserTypes)
    {
        servicesCollection.AddTransient(parserInterface, type);
    }

    servicesCollection.AddTransient<IDataProcessorRepository,DataProcessorRepository>();
    servicesCollection.AddTransient<IDataExtractionService,DataExtractionService>();

}