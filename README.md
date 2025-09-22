1.Clone the repo </br>
2.If using VSCode use this .vscode/tasks.json
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
<br>
3..vscode/launch.json

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
<br>
4.In production the JWT key located in launchSettings.json and the db connection strings should be stored in environment variables,or in a secrets manager.Also the Api/wwwroot/uploads should be included(remove the exclusion from gitignore)

