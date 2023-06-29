# Synchro 4D Schedules External API Application

Copyright © Bentley Systems, Incorporated. All rights reserved.

A sample application that demonstrates how to use the endpoints from the 4D Schedules External API.

## Prerequisites

Detailed in the root directory README.md.

## Development Setup (Visual Studio 2022)

1. Clone Repository
2. Open `4D Schedules API Consumer.sln` and Build
3. Set valid access token with scope 'enterprise' in command line argument `--token` in `Properties/launchSettings.json`.
4. Set valid schedule ID in command line argument `--schedule` in `Properties/launchSettings.json` - this is equivalent to the project ID for the initial release.
5. Set a valid operation value, either `--single` for querying a single endpoint (simply gets information on the given schedule) or `--post` to send a POST request to the Resource Status History endpoint.
   1. The default behaviour with neither of these options set is to demonstrate an example workflow using the 4D Schedules External API - the program will fetch all Resource Status History items then get an individual ID from this list. Afterwards, it will send a POST request to the Resource Status History endpoint which will return a Change Request ID. This ID is then used to query the Change Requests endpoint, to show that the API has picked up this request.
6. Run to debug

## Development Setup (command line)

1. Clone Repository
2. Change directory into cloned folder `cd EsApi4DScheduleSampleApp`
3. Build app with `dotnet build`
4. Change directory into build folder `cd EsApi4DScheduleSampleApp/bin/Debug/net6.0/`
5. Run app with `dotnet EsApi4DScheduleSampleApp.dll --token "{your_token}" --schedule "{your_schedule}"`, additionally add either `--single` or `--post` to the arguments. Note that only one of these can be used at a time.

## How to acquire a token

Detailed in the root directory README.md.

## API documentation

API documentation in OpenAPI V3 format:
* 4D Schedules External API: https://es-api.bentley.com/4dschedule/swagger/index.html

A token can also be acquired from this page for testing.