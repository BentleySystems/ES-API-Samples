# 4D Schedules External API SPA Sample App

Copyright © Bentley Systems, Incorporated. All rights reserved.

A sample application that demonstrates how to use the endpoints from the 4D Schedules External API in an SPA (Single-Page Application).

## Prerequisites

* [Git](https://git.scm.com/)
* [NodeJS and NPM](https://nodejs.org/en/download)
* Optionally an IDE such as [Visual Studio Code](https://code.visualstudio.com/). Using the command line is also possible (and will be used to run the project in VS Code also).
* Ensure that your project (developer.bentley.com and imsoidcui.bentley.com) has the following:
    1. In "Redirect URIs", ensure that `https://localhost:3000/signin-callback` is present. This can be done in either [developer.bentley.com](https://developer.bentley.com) or [imsoidcui.bentley.com](https://imsoidcui.bentley.com).
    2. In "Post logout redirect URIs", ensure that `https://localhost:3000/logout` is present. This can be done in either [developer.bentley.com](https://developer.bentley.com) or [imsoidcui.bentley.com](https://imsoidcui.bentley.com).
    3. In [imsoidcui.bentley.com](https://imsoidcui.bentley.com), ensure that your application has the "Allow Offline Access" option checked and that the Client Grant Type is "Auth Code".

## Development Setup (Visual Studio Code)

1. Clone repository
2. Open the folder in Visual Studio Code
3. Ctrl+J to open a terminal in the IDE which will default to the base folder
4. `npm install --force` in the terminal to install dependencies
5. Run the application. Use `($env:HTTPS = "true") -and (npm start)` to ensure the application runs in HTTPS as IMS does not support login/logout with HTTP.
6. N.B.: When run inside of VS Code (i.e. Powershell), hot-reloading should still work.

## Development Setup (command line)

1. Clone repository
2. Change directory to cloned folder `cd EsApi4DScheduleSPASampleApp`
3. Install dependencies `npm install --force`
5. Run the application
    1. In Powershell, use `($env:HTTPS = "true") -and (npm start)` to ensure the application runs in HTTPS as IMS does not support login/logout with HTTP.
    2. In cmd, use `set HTTPS=true&&npm start`.
    3. In a bash environment (Linux, macOS), use `HTTPS=true npm start`.

## API documentation

API documentation in Swagger:
* 4D Schedules External API: https://es-api.bentley.com/4dschedule/swagger/index.html

A token can also be acquired from this page for testing.
