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
5. Optionally set `--datasourceuri` command line argument. A random data source Uri will be generated otherwise.
6. Run to debug

## Development Setup (command line)

1. Clone Repository
2. Change directory into cloned folder `cd ES-API-Samples`
3. Build app with `dotnet build`
4. Change directory into build folder `cd EsApiProjectsSampleApp/bin/Debug/net6.0/`
5. Run app with `dotnet EsApiProjectsSampleApp.dll --token "{your_token}"`
   1. [How to acquire a token](#how-to-acquire-a-token) is explained below.
   2. Optionally set `--name` command line argument. A random name will be generated otherwise.
   3. Optionally set `--datasourceuri` command line argument. A random data source Uri will be generated otherwise.

## How to acquire a token

Valid access token with scope 'enterprise' is required to access API endpoints. For more technical information about tokens and authentication see: https://developer.bentley.com/apis/overview/authorization.

In order to run this sample app or if you want to develop your own application you'll have to register a client in https://developer.bentley.com/esregister.

### API Client registration steps

 1. If you're not logged in, you'll get redirected to a login page once you go to https://developer.bentley.com/esregister. If you don't have one already, create an account and start the trial. Then go back to api client registration page.
 2. Fill in application name. This is a display name, client id will be generated automatically.
 3. Check the api client details:
    1. Make sure `Enterprise` is checked under *API associations*
    2. Make sure `enterprise` scope is added under *Allowed scopes*
 4. Select an appropriate application type. If you just want to run the sample app `Service` type will be enough. If you don't know which type to choose for a user-facing application check out https://developer.bentley.com/apis/overview/authorization.
 5. Fill in redirect url if application type is not `Service`. This is the url to your application which authentication service will come back to once user is logged in.
 6. Click `Save`
 7. Make sure to copy client secret and close the dialog.
 8. A page should appear with created api client. In order to get tokens you'll also need the client id that should be shown in this window.
 9. You should be able to authenticate now by using client id and secret with the appropriate flow.

### Console commands to get the token via service client credentials

#### Bash

```sh
curl --request POST \
  --url 'https://ims.bentley.com/connect/token' \
  --header 'content-type: application/x-www-form-urlencoded' \
  --data-urlencode grant_type=client_credentials \
  --data-urlencode scope=enterprise \
  --data-urlencode client_id=YOUR_CLIENT_ID \
  --data-urlencode client_secret=YOUR_CLIENT_SECRET
```

#### Powershell

```pwsh
(Invoke-WebRequest -Method 'Post' `
   -Uri 'https://ims.bentley.com/connect/token' `
   -Headers @{ 'content-type' = 'application/x-www-form-urlencoded' } `
   -Body @{ `
      grant_type='client_credentials'; `
      scope='enterprise'; `
      client_id='YOUR_CLIENT_ID'; `
      client_secret='YOUR_CLIENT_SECRET' `
   }).Content
```

## API documentation

API documentation in OpenAPI V3 format:
* Projects API: https://es-api.bentley.com/project/v1/swagger.json
* Work Area Connection API: https://es-api.bentley.com/workarea/v1/swagger.json
