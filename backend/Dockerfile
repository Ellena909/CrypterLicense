# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["CrypterLicense.csproj", "./"]

# Configure NuGet sources and add retry logic for better reliability
RUN dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org --configfile /tmp/nuget.config || true
RUN for i in 1 2 3; do dotnet restore "CrypterLicense.csproj" --disable-parallel --verbosity normal && break || sleep 10; done

# Copy everything else and build
COPY . .
RUN dotnet build "CrypterLicense.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "CrypterLicense.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "CrypterLicense.dll"]