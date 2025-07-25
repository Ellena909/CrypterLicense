# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore dependencies
COPY CrypterLicense.csproj .

# Configure NuGet sources and add retry logic for better reliability
RUN dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org --configfile /tmp/nuget.config || true
RUN for i in 1 2 3; do dotnet restore CrypterLicense.csproj --disable-parallel --verbosity normal && break || sleep 10; done

# Copy everything else and build
COPY . .
RUN dotnet publish CrypterLicense.csproj -c Release -o /app --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Start the application
ENTRYPOINT ["dotnet", "CrypterLicense.dll"]