# -------- Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /FullStackTest/Api
COPY *.sln ./
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# -------- Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Api.dll"]