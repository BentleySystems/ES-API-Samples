# Synchro 4D Schedules External API Application

Copyright © Bentley Systems, Incorporated. All rights reserved.

A sample application that demonstrates how to use the endpoints from the 4D Schedules External API.

## Prerequisites

* [Git](https://git-scm.com/)
* [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0/)
* Optionally an IDE like Visual Studio 2022 or [Visual Studio Code](https://code.visualstudio.com/). It is also possible to use command line.

## Development Setup (Visual Studio 2022)

1. Clone Repository
2. Open `4D Schedules API Consumer.sln` and Build
3. Set valid access token with scope 'enterprise' in command line argument `--token` in `Properties/launchSettings.json`. [How to acquire a token](#how-to-acquire-a-token) is explained below.
4. Set valid schedule ID in command line argument `--schedule` in `Properties/launchSettings.json` - this is equivalent to the project ID for the initial release.
5. Set a valid operation value, either `--single` for querying a single endpoint (simply gets information on the given schedule), `--all` to query every endpoint available, or `--post` to send a POST request to the Resource Status History endpoint. By default, in VS this is set to `--single`, and only one of these options can be used at a time.
6. Run to debug

## Development Setup (command line)

1. Clone Repository
2. Change directory into cloned folder `cd ScheduleAPIConsumer`
3. Build app with `dotnet build`
4. Change directory into build folder `cd ScheduleAPIConsumer/bin/Debug/net6.0/`
5. Run app with `dotnet ScheduleAPIConsumer.dll --token "{your_token}" --schedule "{your_schedule}"`, additionally add either `--single`, `--all`, or `--post` to the arguments. Note that only one of these can be used at a time.
   1. [How to acquire a token](#how-to-acquire-a-token) is explained below.

## How to acquire a token

Valid access token with scope 'enterprise' is required to access API endpoints. For more technical information about tokens and authentication see: https://developer.bentley.com/apis/overview/authorization/.

For testing, a token can be obtained from the API Swagger page: https://es-api.bentley.com/4dschedule/swagger/index.html.

## API documentation

API documentation in OpenAPI V3 format:
* 4D Schedules External API: https://es-api.bentley.com/4dschedule/v1/swagger.json
