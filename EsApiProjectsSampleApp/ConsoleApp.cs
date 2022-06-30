/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
using System.CommandLine;
using System.Reflection;
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
            var dataSourceUriOption = new Option<string>("--datasourceuri",
                getDefaultValue: () => $"http://link-to-pw-wsg.com/ws/v2.8/repositories/datasource/PW_WSG/Project/{Guid.NewGuid()}",
                description: "Work Area Connection data source URI");

            // Add the options to a root command:
            var rootCommand = new RootCommand { tokenOption, nameOption, dataSourceUriOption };

            rootCommand.Description = "Es Api Sample App";

            rootCommand.SetHandler(async (string token, string name, string dataSourceUri) =>
            {
                if (string.IsNullOrWhiteSpace(token) || token == "<Add token here>")
                {
                    Log("--token argument must be set.");
                    return;
                }
                await runAsync(new Arguments(token, name, dataSourceUri), ReadConfiguration());
            }, tokenOption, nameOption, dataSourceUriOption);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args);
        }

        public static Configuration ReadConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
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
