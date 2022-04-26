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
            var tokenOption = new Option<string>("--token", "Authentication token")
            {
                IsRequired = true,
            };
            var nameOption = new Option<string>("--name", getDefaultValue: () => $"Sample_Project_{Guid.NewGuid()}", description: "Project name");

            // Add the options to a root command:
            var rootCommand = new RootCommand { tokenOption, nameOption };

            rootCommand.Description = "Es Api Projects Sample App";

            rootCommand.SetHandler(async (string token, string name) =>
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    Log("--token argument must not be null or empty.");
                    return;
                }
                await runAsync(new Arguments(token, name), ReadConfiguration());
            }, tokenOption, nameOption);

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

        public static void Log(string message, params object?[] args)
        {
            Console.WriteLine("{0}: {1}", DateTime.Now, string.Format(message, args));
        }
    }
}
