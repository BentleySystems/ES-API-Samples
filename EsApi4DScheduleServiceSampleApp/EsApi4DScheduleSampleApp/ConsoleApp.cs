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
            var paginationOption = new Option<string>("--pagination", "Page size to use for pagination - must be between 1-10000.")
            {
                IsRequired = false,
            };
            var paginationEndpoint = new Option<string>("--endpoint", "Endpoint for pagination query")
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
            var rootCommand = new RootCommand { tokenOption, scheduleOption, singleOption, postOption, paginationOption, paginationEndpoint };

            rootCommand.Description = "External Schedule API Sample App";

            rootCommand.SetHandler(async (string token, string schedule, bool single, bool post, string pagination, string endpoint) =>
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

                if (string.IsNullOrWhiteSpace(pagination) && pagination is not null)
                {
                    Log("Pagination was selected but not set. Please set a page size.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(endpoint) && endpoint is not null)
                {
                    Log("Endpoint was selected but not set. Please set an endpoint to use for pagination queries.");
                    return;
                }

                if (pagination is not null)
                {
                    if (int.TryParse(pagination, out var num))
                    {
                        if (num < 1 || num > 10000)
                        {
                            Log("Page size was not within the supported range (1-10000). Please provide a number within the appropriate range.");
                            return;
                        }
                    } else
                    {
                        Log("Page size given was not a number. Please provide a number within the appropriate range (1-10000).");
                        return;
                    }
                }

                if (pagination is not null && endpoint is null)
                {
                    Log("Pagination was selected but endpoint was not provided. Please provide an endpoint to use for pagination queries.");
                    return;
                }

                if (pagination is null && endpoint is not null)
                {
                    Log("Endpoint was selected without pagination being set. Please provide the pagination option with an apppropriate page size (1-10000).");
                    return;
                }

                    await runAsync(new Arguments(token, schedule, single, post, pagination, endpoint), ReadConfiguration());
            }, tokenOption, scheduleOption, singleOption, postOption, paginationOption, paginationEndpoint);

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
