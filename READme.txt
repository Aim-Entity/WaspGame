PREREQUISITES
- .NET 10 SDK
- Node.js (LTS 20+) and npm
- Trusted local HTTPS dev certificate (see below)

No external database or services are required; appsettings.json uses defaults out of the box.

OPTION 1: RUN EVERYTHING TOGETHER (recommended)

The server automatically launches the React dev server for you via the SpaProxy package.

1. Install client dependencies (first time only):

     cd WaspEngine/waspengine.client
     npm install

2. Open WaspGame.slnx in Visual Studio, OR run from the CLI:

     dotnet run --project WaspEngine/WaspEngine.Server/WaspEngine.Server.csproj --launch-profile https

3. This starts:
     - API server:  https://localhost:7011/swagger  (and http://localhost:5296/swagger)
     - React app is auto-started and proxied through the server

4. First time only - trust the dev certificate if prompted:
     dotnet dev-certs https --trust


OPTION 2: RUN SERVER AND CLIENT SEPARATELY

1. Install client dependencies (first time only):

     cd WaspEngine/waspengine.client
     npm install

2. Start the API server:
     dotnet run --project WaspEngine/WaspEngine.Server/WaspEngine.Server.csproj --launch-profile https
   -> API available at https://localhost:7011

3. Start the React dev server (in a separate terminal):
     cd WaspEngine/waspengine.client
     npm run dev

   -> React app available at https://localhost:52144
   -> Vite auto-generates a dev HTTPS cert on first run (stored in %APPDATA%/ASP.NET/https)
   -> Any /api/* requests are proxied automatically to the API server

4. Open the app in your browser:

     https://localhost:52144   (React app, with API calls proxied)
     https://localhost:7011/swagger    (API directly / Swagger)


ENSURE .env FILE IS CREATED IN THE ROOT OF waspengine.client

.env VITE VARIABLES:

VITE_API_BASE_URL=/api/v1

VITE_GAME_POLL_INTERVAL_MS=500

VITE_API_DEV_SERVER_PORT=52144

VITE_API_ASPNETCORE_URLS=https://localhost:7011