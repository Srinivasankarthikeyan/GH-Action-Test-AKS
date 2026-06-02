# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["DotnetVmDeployDemo.sln", "./"]
COPY ["DemoApi/DemoApi.csproj", "DemoApi/"]

RUN dotnet restore "DemoApi/DemoApi.csproj"

COPY ["DemoApi/", "DemoApi/"]

WORKDIR /src/DemoApi
RUN dotnet publish "DemoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

USER $APP_UID

WORKDIR /app
COPY --from=build /app/publish ./

EXPOSE 80

ENTRYPOINT ["dotnet", "DemoApi.dll"]