# ES-API Sample Apps

Copyright © Bentley Systems, Incorporated. All rights reserved.

A repository for sample applications which utilise aspects of Bentley's ES-API. This README details processes which are the same among the subprojects.
* [ES-API Projects](/EsApiProjectsSampleApp/) - Demonstrates how to create, query, and delete a Project using the Enterprise Systems Project API, also dealing with billing country endpoints and data center endpoint usage.
* [ES-API WorkArea](/EsApiProjectsSampleApp/) - ProjectWise Web Connections API for mapping Work Area Connections from Project Wise Design Integration to iTwin Projects.
* [ES-API 4D Schedules Service](/EsApi4DScheduleServiceSampleApp/) - Demonstrates an example workflow of acquiring a Resource Status History item and changing the Date value within said item using the 4D Schedules External API.
* [ES-API 4D Schedules SPA](/EsApi4DScheduleSPASampleApp/) - Demonstrates an example workflow of acquiring a Resource Status History item and changing the Date value within said item using the 4D Schedules External API.

## Prerequisites

* [Git](https://git-scm.com/)
* (For non-web applications) [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0/)
* (For web applications) [NodeJS and NPM](https://nodejs.org/en/download)
* Optionally an IDE like Visual Studio 2022 or [Visual Studio Code](https://code.visualstudio.com/). It is also possible to use command line.

## How to acquire a token

Valid access token with scope 'enterprise' is required to access API endpoints. For more technical information about tokens and authentication see: https://developer.bentley.com/apis/overview/authorization.

In order to run this sample app or if you want to develop your own application you'll have to register a client in https://developer.bentley.com/esregister.

### API Client registration steps

 1. If you're not logged in, you'll get redirected to a login page once you go to https://developer.bentley.com/esregister. If you don't have one already, create an account and start the trial. Then go back to api client registration page.
 2. Fill in application name. This is a display name, client id will be generated automatically.
 3. Check the api client details:
    1. Make sure `Enterprise` is checked under *API associations*
    2. Make sure `enterprise` scope is added under *Allowed scopes*
    3. For web apps, make sure `Allow Offline Access` is checked.
 4. Select an appropriate application type. If you just want to run the sample app get user token from https://microfrontenddeveussa01.z13.web.core.windows.net/esapi. 
 5. If you don't know which type to choose for a user-facing application check out https://developer.bentley.com/apis/overview/authorization. We highly recommend to use either SPA or Web App type as the token generated from these apps will be a user token and not a service token(if application type is `Service`). If you want to use `Service`type app, then you need to make a ServiceNow request with the clientId and email(clientId@apps.imsoidc.bentley.com) to add this service identity user to your organization along with CONNECT Services Admin role as this user needs to access all projects within the organization and able to create new projects.
 6. Fill in redirect url if application type is not `Service`. This is the url to your application which authentication service will come back to once user is logged in.
 7. Click `Save`
 8. Make sure to copy client secret and close the dialog. Client secret is not required if the application is of SPA type.
 9. A page should appear with created api client. In order to get tokens you'll also need the client id that should be shown in this window.
 10. You should be able to authenticate now by using client id and secret with the appropriate flow.

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
