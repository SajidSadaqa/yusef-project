# Restore & publish stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Create a NuGet.Config to prevent Windows path issues
RUN echo '<?xml version="1.0" encoding="utf-8"?>\n\
<configuration>\n\
  <packageSources>\n\
    <clear />\n\
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />\n\
  </packageSources>\n\
  <fallbackPackageFolders>\n\
    <clear />\n\
  </fallbackPackageFolders>\n\
</configuration>' > NuGet.Config

COPY ShipmentTracking.sln ./
COPY src/ShipmentTracking.Domain/ShipmentTracking.Domain.csproj src/ShipmentTracking.Domain/
COPY src/ShipmentTracking.Application/ShipmentTracking.Application.csproj src/ShipmentTracking.Application/
COPY src/ShipmentTracking.Infrastructure/ShipmentTracking.Infrastructure.csproj src/ShipmentTracking.Infrastructure/
COPY src/ShipmentTracking.WebApi/ShipmentTracking.WebApi.csproj src/ShipmentTracking.WebApi/
COPY tests/ShipmentTracking.Application.Tests/ShipmentTracking.Application.Tests.csproj tests/ShipmentTracking.Application.Tests/
COPY tests/ShipmentTracking.Domain.Tests/ShipmentTracking.Domain.Tests.csproj tests/ShipmentTracking.Domain.Tests/
COPY tests/ShipmentTracking.Integration.Tests/ShipmentTracking.Integration.Tests.csproj tests/ShipmentTracking.Integration.Tests/
RUN dotnet restore ShipmentTracking.sln --configfile NuGet.Config

COPY . .
# Don't use --no-restore as it still tries to resolve package assets
# The NuGet.Config will be used automatically
RUN dotnet publish src/ShipmentTracking.WebApi/ShipmentTracking.WebApi.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ShipmentTracking.WebApi.dll"]
