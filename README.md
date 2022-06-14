# Enterprise Systems API Sample Application

Copyright © Bentley Systems, Incorporated. All rights reserved.

A sample application that demonstrates how to create, query and delete a Project using the Enterprise Systems Project API. It also demonstrates Enterprise Systems Work Area Connection API, billing country endpoint and data center endpoint usage.

## Prerequisites

* [Git](https://git-scm.com/)
* [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0/)
* Optionally an IDE like Visual Studio 2022 or [Visual Studio Code](https://code.visualstudio.com/). It is also possible to use command line.

## Development Setup (Visual Studio 2022)

1. Clone Repository
2. Open `ES API Projects Sample App.sln` and Build
3. Set valid access token with scope 'enterprise' in command line argument `--token` in `Properties/launchSettings.json`. [How to acquire a token](#how-to-acquire-a-token) is explained below.
4. Optionally set `--name` command line argument. A random name will be generated otherwise.
5. Optionally set `--datasourceuri` command line argument. A random data sourse Uri will be generated otherwise.
6. Run to debug

## Development Setup (command line)

1. Clone Repository
2. Change directory into cloned folder `cd ES-API-Projects-Sample`
3. Build app with `dotnet build`
4. Change directory into build folder `cd EsApiProjectsSampleApp/bin/Debug/net6.0/`
5. Run app with `dotnet EsApiProjectsSampleApp.dll --token "{your_token}"`
   1. [How to acquire a token](#how-to-acquire-a-token) is explained below.
   2. Optionally set `--name` command line argument. A random name will be generated otherwise.
   3. Optionally set `--datasourceuri` command line argument. A random data sourse Uri will be generated otherwise.

## How to acquire a token

Valid access token with scope 'enterprise' is required to access API endpoints. For more technical information about tokens and authentication see: https://developer.bentley.com/apis/overview/authorization/.

For testing token could be obtained from API Swagger page: https://esapi-projects-eus.bentley.com/swagger/index.html.

For application need to register client and use its credentials to get access token. 

## API documentation

API documentation in OpenAPI V3 format:
* Projects API: https://es-api.bentley.com/project/preview/swagger.json
* Work Area Connection API: https://es-api.bentley.com/workarea/preview/swagger.json
