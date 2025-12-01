# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY src/N5.Permissions.Domain/N5.Permissions.Domain.csproj src/N5.Permissions.Domain/
COPY src/N5.Permissions.Application/N5.Permissions.Application.csproj src/N5.Permissions.Application/
COPY src/N5.Permissions.Infrastructure/N5.Permissions.Infrastructure.csproj src/N5.Permissions.Infrastructure/
COPY src/N5.Permissions.API/N5.Permissions.API.csproj src/N5.Permissions.API/

RUN dotnet restore src/N5.Permissions.API/N5.Permissions.API.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/src/N5.Permissions.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "N5.Permissions.API.dll"]

