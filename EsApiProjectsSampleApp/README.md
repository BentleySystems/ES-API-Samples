# Enterprise Systems API Sample Application

Copyright © Bentley Systems, Incorporated. All rights reserved.

A sample application that demonstrates how to create, query and delete a Project using the Enterprise Systems Project API. It also demonstrates Enterprise Systems Work Area Connection API, billing country endpoint and data center endpoint usage.

## Prerequisites

Detailed in the [root directory README.md](../README.md).

## Development Setup (Visual Studio 2022)

1. Clone Repository
2. Open `ES API Projects Sample App.sln` and Build
3. Set valid access token with scope 'enterprise' in command line argument `--token` in `Properties/launchSettings.json`.
4. Optionally set `--name` command line argument. A random name will be generated otherwise.
5. Optionally set `--datasourceuri` command line argument. A random data source Uri will be generated otherwise.
6. Run to debug

## Development Setup (command line)

1. Clone Repository
2. Change directory into cloned folder `cd ES-API-Samples`
3. Build app with `dotnet build`
4. Change directory into build folder `cd EsApiProjectsSampleApp/bin/Debug/net6.0/`
5. Run app with `dotnet EsApiProjectsSampleApp.dll --token "{your_token}"`
   1. Optionally set `--name` command line argument. A random name will be generated otherwise.
   2. Optionally set `--datasourceuri` command line argument. A random data source Uri will be generated otherwise.

## How to acquire a token

Detailed in the [root directory README.md](../README.md).

## API documentation

API documentation in OpenAPI V3 format:
* Projects API: https://es-api.bentley.com/project/v1/swagger.json
* Work Area Connection API: https://es-api.bentley.com/workarea/v1/swagger.json