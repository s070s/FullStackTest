# -------- Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only the project file first (better cache)
COPY Api.csproj ./
RUN dotnet restore Api.csproj

# copy the rest of the Api source
COPY . .

# publish the project (NOT the solution)
RUN dotnet publish Api.csproj -c Release -o /app/publish /p:UseAppHost=false --no-restore

# -------- Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Api.dll"]