/*---------------------------------------------------------------------------------------------
* Copyright (c) Bentley Systems, Incorporated. All rights reserved.
* See LICENSE.md in the project root for license terms and full copyright notice.
*--------------------------------------------------------------------------------------------*/
using System.CommandLine;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace EsApi4DScheduleSampleApp
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
            var scheduleOption = new Option<string>("--schedule", "Schedule ID to query from")
            {
                IsRequired = true,
            };
            var paginationOption = new Option<string>("--pagination", "Page size (integer) to use for pagination against the Resources User Field Values endpoint - /4dschedules/v1/schedules/{schedule_id}/resources/userFieldValues")
            {
                IsRequired = false,
            };
            var singleOption = new Option<bool>("--single", "Queries a single endpoint - /4dschedule/v1/schedules/{schedule_id}")
            {
                IsRequired = false,
            };
            var postOption = new Option<bool>("--post", "Send a POST request to the Resource Status History endpoint")
            {
                IsRequired = false,
            };

            // Add the options to a root command:
            var rootCommand = new RootCommand { tokenOption, scheduleOption, singleOption, postOption, paginationOption };

            rootCommand.Description = "External Schedule API Sample App";

            rootCommand.SetHandler(async (string token, string schedule, bool single, bool post, string pagination) =>
            {
                if (string.IsNullOrWhiteSpace(token) || token == "<Add token here>")
                {
                    Log("--token argument must be set.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(schedule) || schedule == "<Add schedule ID here>")
                {
                    Log("--schedule argument must be set.");
                    return;
                }

                if (single ? (post || pagination is not null) : (post && pagination is not null))
                {
                    Log("Cannot set multiple functionality arguments (POST, single, pagination) at once. Select only one of these options.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(pagination) || pagination == "<Add page size here>")
                {
                    Log("Pagination was selected but not set. Please set a page size.");
                    return;
                }

                    await runAsync(new Arguments(token, schedule, single, post, pagination), ReadConfiguration());
            }, tokenOption, scheduleOption, singleOption, postOption, paginationOption);

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
                ServiceHost: new Uri(configuration[nameof(Configuration.ServiceHost)]));
        }

        public static void Log(string message, params object?[] args)
        {
            Console.WriteLine("{0}: {1}", DateTime.Now, string.Format(message, args));
        }
    }
}
