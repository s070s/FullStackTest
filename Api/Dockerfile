# -------- Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only the API project first (better cache) + restore just it
COPY Api.csproj ./
RUN dotnet restore

# now copy the rest of the Api project
COPY . .

# publish only the API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# -------- Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run
WORKDIR /app
# Render provides PORT; bind Kestrel to it
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "Api.dll"]