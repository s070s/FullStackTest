1.Clone the repo </br>
2.If using VSCode use this .vscode/tasks.json

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "InstallAndBuildAPI",
            "type": "shell",
            "command": "dotnet restore; dotnet build",
            "options": {
                "cwd": "${workspaceFolder}/Api"
            }
        },
        {
            "label": "InstallAndBuildFrontend",
            "type": "shell",
            "command": "npm install; npm run build",
            "options": {
                "cwd": "${workspaceFolder}/Frontend"
            }
        },
        {
            "label": "InstallAndBuildAll",
            "dependsOn": [
                "InstallAndBuildAPI",
                "InstallAndBuildFrontend"
            ],
            "dependsOrder": "parallel"
        },
        {
            "label": "RunAPI",
            "type": "shell",
            "command": "dotnet run",
            "options": {
                "cwd": "${workspaceFolder}/Api"
            }
        },
        {
            "label": "RunFrontend",
            "type": "shell",
            "command": "npm run dev",
            "options": {
                "cwd": "${workspaceFolder}/Frontend"
            }
        },
        {
            "label": "RunBoth",
            "dependsOn": [
                "RunAPI",
                "RunFrontend"
            ],
            "dependsOrder": "parallel"
        },
        {
            "label": "CleanAPI",
            "type": "shell",
            "command": "Remove-Item -Recurse -Force obj -ErrorAction SilentlyContinue; Remove-Item -Recurse -Force bin -ErrorAction SilentlyContinue",
            "options": {
                "cwd": "${workspaceFolder}/Api"
            },
        },
        {
            "label": "CleanFrontend",
            "type": "shell",
            "command": "Remove-Item -Recurse -Force dist -ErrorAction SilentlyContinue; Remove-Item -Recurse -Force build -ErrorAction SilentlyContinue",
            "options": {
                "cwd": "${workspaceFolder}/Frontend"
            },
        },
        {
            "label": "CleanAll",
            "dependsOn": [
                "CleanAPI",
                "CleanFrontend"
            ],
            "dependsOrder": "parallel"
        },
        {
            "label": "RecreateDatabase",
            "dependsOn": [
                "CleanMigrationsAndDB",
                "AddMigrationAndUpdate"
            ],
            "dependsOrder": "sequence"
        },
        {
            "label": "CleanMigrationsAndDB",
            "type": "shell",
            "command": "Remove-Item -Recurse -Force Migrations -ErrorAction SilentlyContinue; Remove-Item -Force *.db,*.sqlite -ErrorAction SilentlyContinue",
            "options": {
                "cwd": "${workspaceFolder}/Api"
            }
        },
        {
            "label": "UpdateDatabase",
            "type": "shell",
            "command": "dotnet ef database update",
            "options": {
                "cwd": "${workspaceFolder}/Api"
            }
        },
        {
            "label": "AddMigrationAndUpdate",
            "type": "shell",
            "command": "dotnet ef migrations add ${input:migrationName}; dotnet ef database update",
            "options": {
                "cwd": "${workspaceFolder}/Api"
            }
        }
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
3.launch.json

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

