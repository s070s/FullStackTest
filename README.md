1.Clone the repo </br>
2.If there are issues with the verification of the certification run this dotnet dev-certs https --trust</br>
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
            "problemMatcher": []
        },
        {
            "label": "CleanFrontend",
            "type": "shell",
            "command": "Remove-Item -Recurse -Force dist -ErrorAction SilentlyContinue; Remove-Item -Recurse -Force build -ErrorAction SilentlyContinue",
            "options": {
                "cwd": "${workspaceFolder}/Frontend"
            },
            "problemMatcher": []
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
            "label": "InitialMigrationAndUpdateDB",
            "type": "shell",
            "command": "dotnet ef migrations add InitialCreate; dotnet ef database update",
            "options": {
                "cwd": "${workspaceFolder}/Api"
            }
        },
    ]
}
```

