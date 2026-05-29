# Use the official .NET 8 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["DotnetVmDeployDemo.sln", "./"]
COPY ["DemoApi/DemoApi.csproj", "DemoApi/"]

# Restore dependencies
RUN dotnet restore "DemoApi/DemoApi.csproj"

# Copy the rest of the source code
COPY ["DemoApi/", "DemoApi/"]

# Build and publish the application
WORKDIR /src/DemoApi
RUN dotnet publish "DemoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the smaller ASP.NET Core runtime image for execution
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Expose the default port used by ASP.NET Core
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "DemoApi.dll"]
