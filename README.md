# FullStackTest
## 0. Description
A personal educational project to improve my understanding
supposedly a way for trainees to support their fitness instructors ala patreon,onlyfans. As of Sept 2025 very WIP
## 1. Clone the Repo

Clone this repository to your local machine.

---

## 2. Manual Setup Instructions

Open a terminal and run the following commands step by step:

```sh
cd FullStackTest
cd Api
dotnet restore
dotnet build
dotnet ef migrations add FirstMigration
dotnet ef database update
dotnet run
```

### Running the automated tests

```sh
cd FullStackTest
dotnet test Api/Api.sln
```

Open a new terminal for the frontend:

```sh
cd FullStackTest/Frontend
npm install
npm run build
npm run dev
```

---

## 3. VSCode Tasks

If using VSCode, you can use the provided `.vscode/tasks.json` to automate common tasks.  
**Recommended order:**

- **Both-InstallAndBuild**: Installs packages and builds both projects.
- **DB-AddMigrationAndUpdate**: Initializes the database and applies the first migration for the API.
- **Both-Run**: Runs both the API and the Frontend at once.

### Example tasks.json snippet
```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "API-InstallAndBuild",
            "type": "shell",
            "command": "dotnet restore; dotnet build",
            "options": {
                "cwd": "${workspaceFolder}/${input:apiPath}"
            }
        },
        {
            "label": "Frontend-InstallAndBuild",
            "type": "shell",
            "command": "npm install; npm run build",
            "options": {
                "cwd": "${workspaceFolder}/${input:frontendPath}"
            }
        },
        {
            "label": "Both-InstallAndBuild",
            "dependsOn": [
                "API-InstallAndBuild",
                "Frontend-InstallAndBuild"
            ],
            "dependsOrder": "parallel"
        },
        {
            "label": "API-Run",
            "type": "shell",
            "command": "dotnet run",
            "options": {
                "cwd": "${workspaceFolder}/${input:apiPath}"
            }
        },
        {
            "label": "Frontend-Run",
            "type": "shell",
            "command": "npm run dev",
            "options": {
                "cwd": "${workspaceFolder}/${input:frontendPath}"
            }
        },
        {
            "label": "Both-Run",
            "dependsOn": [
                "API-Run",
                "Frontend-Run"
            ],
            "dependsOrder": "parallel"
        },
        {
            "label": "API-Clean",
            "type": "shell",
            "command": "Remove-Item -Recurse -Force obj -ErrorAction SilentlyContinue; Remove-Item -Recurse -Force bin -ErrorAction SilentlyContinue",
            "options": {
                "cwd": "${workspaceFolder}/${input:apiPath}"
            },
        },
        {
            "label": "Frontend-Clean",
            "type": "shell",
            "command": "Remove-Item -Recurse -Force dist -ErrorAction SilentlyContinue; Remove-Item -Recurse -Force build -ErrorAction SilentlyContinue",
            "options": {
                "cwd": "${workspaceFolder}/${input:frontendPath}"
            },
        },
        {
            "label": "Both-Clean",
            "dependsOn": [
                "API-Clean",
                "Frontend-Clean"
            ],
            "dependsOrder": "parallel"
        },
        {
            "label": "DB-Recreate",
            "dependsOn": [
                "DB-CleanDbAndMigrations",
                "DB-AddMigrationAndUpdate"
            ],
            "dependsOrder": "sequence"
        },
        {
            "label": "DB-CleanDbAndMigrations",
            "type": "shell",
            "command": "Remove-Item -Recurse -Force Migrations -ErrorAction SilentlyContinue; Remove-Item -Force *.db,*.sqlite -ErrorAction SilentlyContinue",
            "options": {
                "cwd": "${workspaceFolder}/${input:apiPath}"
            }
        },
        {
            "label": "DB-UpdateDatabase",
            "type": "shell",
            "command": "dotnet ef database update",
            "options": {
                "cwd": "${workspaceFolder}/${input:apiPath}"
            }
        },
        {
            "label": "DB-AddMigrationAndUpdate",
            "type": "shell",
            "command": "dotnet ef migrations add ${input:migrationName}; dotnet ef database update",
            "options": {
                "cwd": "${workspaceFolder}/${input:apiPath}"
            }
        },
        {
            "label": "List-NpmPackages",
            "type": "shell",
            "command": "npm list --depth=0",
            "options": {
                "cwd": "${workspaceFolder}/${input:frontendPath}"
            }
        },
        {
            "label": "List-GlobalNpmPackages",
            "type": "shell",
            "command": "npm list -g --depth=0",
            "options": {
                "cwd": "${workspaceFolder}/${input:frontendPath}"
            }
        },
        {
            "label": "List-NugetPackages",
            "type": "shell",
            "command": "dotnet list package",
            "options": {
                "cwd": "${workspaceFolder}/${input:apiPath}"
            }
        },
        {
            "label": "List-GlobalNugetPackages",
            "type": "shell",
            "command": "dotnet nuget list source"
        },
        {
            "label": "List-DotnetGlobalTools",
            "type": "shell",
            "command": "dotnet tool list -g"
        },
        {
            "label": "List-NodeVersion",
            "type": "shell",
            "command": "node -v",
            "options": {
                "cwd": "${workspaceFolder}/${input:frontendPath}"
            }
        },
        {
            "label": "List-NpmVersion",
            "type": "shell",
            "command": "npm -v",
            "options": {
                "cwd": "${workspaceFolder}/${input:frontendPath}"
            }
        },
        {
            "label": "List-DotnetSdks",
            "type": "shell",
            "command": "dotnet --list-sdks",
            "options": {
                "cwd": "${workspaceFolder}/${input:apiPath}"
            }
        },
        {
            "label": "List-DotnetRuntimes",
            "type": "shell",
            "command": "dotnet --list-runtimes",
            "options": {
                "cwd": "${workspaceFolder}/${input:apiPath}"
            }
        },
        {
            "label": "ExecutionPolicy: Set RemoteSigned (CurrentUser)",
            "type": "shell",
            "command": "powershell",
            "args": [
                "-Command",
                "Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned -Force"
            ],
            "problemMatcher": []
        },
        {
            "label": "ExecutionPolicy: Reset to Undefined (CurrentUser)",
            "type": "shell",
            "command": "powershell",
            "args": [
                "-Command",
                "Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Undefined -Force"
            ],
            "problemMatcher": []
        },
        {
            "label": "ExecutionPolicy: List All Policies",
            "type": "shell",
            "command": "powershell",
            "args": [
                "-Command",
                "Get-ExecutionPolicy -List"
            ],
            "problemMatcher": []
        },
        {
            "label": "Test:API",
            "type": "shell",
            "command": "dotnet",
            "args": [
                "test",
                "Api.Tests/Api.Tests.csproj"
            ],
            "group": "test",
            "problemMatcher": []
        }
    ],
    "inputs": [
        {
            "id": "migrationName",
            "type": "promptString",
            "description": "Enter the migration name",
            "default": "NewMigration"
        },
        {
            "id": "apiPath",
            "type": "pickString",
            "description": "Enter the path to the Api folder",
            "options": [
                "Api"
            ],
            "default": "Api"
        },
        {
            "id": "frontendPath",
            "type": "pickString",
            "description": "Enter the path to the Frontend folder",
            "options": [
                "Frontend"
            ],
            "default": "Frontend"
        }
    ]
}
```
## 4. VSCode Debugging

The `.vscode/launch.json` file provides launch configurations for debugging:

- **Backend: .NET Core Attach**: Attaches to the running API process (ex. Api.exe).
- **Vite: Frontend Dev Server**: Runs the frontend dev server.
- **Vite: Frontend Debug (Chrome)**: Launches Chrome for frontend debugging.

You can also use the **Frontend: Dev & Debug** compound configuration to start both the dev server and Chrome debugger together.

---

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            /*
            Attach to the running .NET Core process associated with the .NET backend
            which exists in bin/Debug/net9.0/
            */
            "name": "Backend:.NET Core Attach",
            "type": "coreclr",
            "request": "attach"
        },
        {
            "name": "Vite: Frontend Dev Server",
            "type": "node",
            "request": "launch",
            "cwd": "${workspaceFolder}/Frontend",
            "runtimeExecutable": "npm",
            "runtimeArgs": [
                "run",
                "dev"
            ],
        },
        {
            "name": "Vite: Frontend Debug (Chrome)",
            "type": "chrome",
            "request": "launch",
            "url": "http://localhost:5173",
            "webRoot": "${workspaceFolder}/Frontend/src",
        }
    ],
    "compounds": [
        {
        /*
        Run this compound to start a browser that updates changes in the Frontend and allows debugging
        of the Frontend
        */
            "name": "Frontend: Dev & Debug",
            "configurations": [
                "Vite: Frontend Dev Server",
                "Vite: Frontend Debug (Chrome)"
            ]
        }
    ]
}
```


## 5. Important For "Development"
.env file (at the root of the Frontend React Project)
VITE_API_DEV_URL=http://localhost:5203

## 6. Important For "Production"- API on Render
-Env vars for API
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="Data Source=app.db"
Jwt__Issuer="TestApi"
Frontend__Url="https://full-stack-test-beta.vercel.app"
Jwt__Key="the-much-longer-secret-key-which-is-at-least-32-characters-long!"
JWT__AccessTokenMinutes=30
JWT__RefreshTokenDays=7

-DockerFile and dockerignore included in the API Root for its deployment

## 7. Authentication Token Lifetimes and Other information
- Access token (short-lived JWT, e.g. 30 minutes) kept in memory only (React state / AuthContext) attached to Authorization header  for security reasons If expired or near expiry the client obtains a new access token via the refresh flow.

- Refresh token (long-lived, e.g. 7 days) set by the server as an HttpOnly, Secure cookie (cookie name: `refreshToken`). unreadable with javascript. Client calls `POST /refresh` with fetch option `credentials: "include"` (no token in request body). The browser automatically sends the HttpOnly cookie; the server validates and rotates the refresh token and returns a new access token in JSON.
- Flows
  - Login: `POST /login` → server returns access token JSON and sets an HttpOnly refresh cookie.
  - Silent reload: frontend calls `POST /refresh` once on startup (credentials included) to repopulate in-memory access token after a full page reload.
  - Refresh: client calls `POST /refresh` (credentials included) to rotate tokens when access token is expired/close to expiry.
  - Logout: client calls `POST /logout` on the API origin with credentials to clear the refresh cookie; client clears in-memory access token and user state.



## 8. Important For "Production"- Frontend on Vercel
-Env vars for frontend
VITE_API_URL=https://fullstacktest-nokq.onrender.com/api

## 9. Seeded admin account credentials
username:admin
password:admin1234567

## 10. Development URLs for Reference
Frontend:http://localhost:5173/
API:http://localhost:5203

## 11. Production URLs for Reference
Frontend:https://full-stack-test-beta.vercel.app/
API:https://fullstacktest-nokq.onrender.com/api

## 12. Other Notes
-Uploading Static Files is not supported on Production as persistent storage on Render(the service where the api is deployed) requires a subscription beyond the free plan
-The deployed API on Render slows down after 15 mins of inactivity due to the free tier.