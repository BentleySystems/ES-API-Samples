using System.CommandLine;
using Microsoft.Extensions.Configuration;

namespace EsApiProjectsSampleApp
{
    public delegate Task RunAppAsync(Arguments arguments, Configuration configuration);

    public static class ConsoleApp
    {
        public static Task<int> RunAsync(string[] args, RunAppAsync runAsync)
        {
            // Console App options
            var tokenOption = new Option<string>("--token", "Authentication token");
            var nameOption = new Option<string>("--name", getDefaultValue: () => $"Sample_Project_{Guid.NewGuid()}", description: "Project name");

            // Add the options to a root command:
            var rootCommand = new RootCommand { tokenOption, nameOption };

            rootCommand.Description = "Es Api Projects Sample App";

            rootCommand.SetHandler((string token, string name) => runAsync(new Arguments(token, name), ReadConfiguration()), tokenOption, nameOption);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args);
        }

        public static Configuration ReadConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            return new Configuration(
                ServiceHost: new Uri(configuration[nameof(Configuration.ServiceHost)]),
                ProjectType: configuration[nameof(Configuration.ProjectType)]);
        }
    }
}
