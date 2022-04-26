# ES API Projects Sample App

Copyright © Bentley Systems, Incorporated. All rights reserved.

A sample application that demonstrates how to create, query and delete a Project using the ES Project API. It also shows billing country and data center endpoint usage.

## Prerequisites

* [Git](https://git-scm.com/)
* [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0/)
* Optionally an IDE like Visual Studio 2022 or [Visual Studio Code](https://code.visualstudio.com/). You can also use command line if you whish

## Development Setup (Visual Studio 2022)

1. Clone Repository
2. Open `ES API Projects Sample App.sln` and Build
3. Set user token command line argument in `Properties/launchSettings.json`. How to acquire a token is explained [here](#how-to-acquire-a-token)
4. Optionally set `--name` command line argument. A random name will be generated otherwise.
5. Run to debug

## Development Setup (command line)

1. Clone Repository
2. Change directory into cloned folder `cd ES-API-Projects-Sample`
3. Build app with `dotnet build`
4. Change directory into build folder `cd EsApiProjectsSampleApp/bin/Debug/net6.0/`
5. Run app with `dotnet EsApiProjectsSampleApp.dll --token "{your_token}"`
   1. How to acquire a token is explained [here](#how-to-acquire-a-token)
   2. Optionally set `--name` command line argument. A random name will be generated otherwise.

## How to acquire a token
<span style="color:red">TODO: Explain how to get a user 