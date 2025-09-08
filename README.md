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
                "cwd": "${workspaceFolder}/Api"
            }
        },
        {
            "label": "Frontend-InstallAndBuild",
            "type": "shell",
            "command": "npm install; npm run build",
            "options": {
                "cwd": "${workspaceFolder}/Frontend"
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
                "cwd": "${workspaceFolder}/Api"
            }
        },
        {
            "label": "Frontend-Run",
            "type": "shell",
            "command": "npm run dev",
            "options": {
                "cwd": "${workspaceFolder}/Frontend"
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
                "cwd": "${workspaceFolder}/Api"
            },
        },
        {
            "label": "Frontend-Clean",
            "type": "shell",
            "command": "Remove-Item -Recurse -Force dist -ErrorAction SilentlyContinue; Remove-Item -Recurse -Force build -ErrorAction SilentlyContinue",
            "options": {
                "cwd": "${workspaceFolder}/Frontend"
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
                "cwd": "${workspaceFolder}/Api"
            }
        },
        {
            "label": "DB-UpdateDatabase",
            "type": "shell",
            "command": "dotnet ef database update",
            "options": {
                "cwd": "${workspaceFolder}/Api"
            }
        },
        {
            "label": "DB-AddMigrationAndUpdate",
            "type": "shell",
            "command": "dotnet ef migrations add ${input:migrationName}; dotnet ef database update",
            "options": {
                "cwd": "${workspaceFolder}/Api"
            }
        },
        {
            "label": "List-NpmPackages",
            "type": "shell",
            "command": "npm list --depth=0",
            "options": {
                "cwd": "${workspaceFolder}/Frontend"
            }
        },
        {
            "label": "List-GlobalNpmPackages",
            "type": "shell",
            "command": "npm list -g --depth=0",
            "options": {
                "cwd": "${workspaceFolder}/Frontend"
            }
        },
        {
            "label": "List-NugetPackages",
            "type": "shell",
            "command": "dotnet list package",
            "options": {
                "cwd": "${workspaceFolder}/Api"
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
                "cwd": "${workspaceFolder}/Frontend"
            }
        },
        {
            "label": "List-NpmVersion",
            "type": "shell",
            "command": "npm -v",
            "options": {
                "cwd": "${workspaceFolder}/Frontend"
            }
        },
        {
            "label": "List-DotnetSdks",
            "type": "shell",
            "command": "dotnet --list-sdks",
            "options": {
                "cwd": "${workspaceFolder}/Api"
            }
        },
        {
            "label": "List-DotnetRuntimes",
            "type": "shell",
            "command": "dotnet --list-runtimes",
            "options": {
                "cwd": "${workspaceFolder}/Api"
            }
        },
    ],
    "inputs": [
        {
            "id": "migrationName",
            "type": "promptString",
            "description": "Enter the migration name",
            "default": "NewMigration"
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
                "name": "C#: Launch API (Backend)",
                "type": "dotnet",
                "request": "launch",
                "projectPath": "${workspaceFolder}/Api/Api.csproj",
                "preLaunchTask": "RunAPI"
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
    ]
}
```

