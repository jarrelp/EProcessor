#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG NET_IMAGE=8.0-jammy
FROM mcr.microsoft.com/dotnet/aspnet:${NET_IMAGE} AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:${NET_IMAGE} AS build
WORKDIR /src

# Create this "restore-solution" section by running ./Create-DockerfileSolutionRestore.ps1, to optimize build cache reuse
COPY ["src/BuildingBlocks/EventBus/EventBus.csproj", "src/BuildingBlocks/EventBus/"]
COPY ["src/BuildingBlocks/Healthchecks/Healthchecks.csproj", "src/BuildingBlocks/Healthchecks/"]
COPY ["src/Services/FakeFetch.API/FakeFetch.API.csproj", "src/Services/FakeFetch.API/"]
COPY ["src/Web/Webstatus/Webstatus.csproj", "src/Web/Webstatus/"]
COPY ["eProcessor.sln", "./"]
RUN dotnet restore "eProcessor.sln"

COPY . .
WORKDIR "/src/src/Web/Webstatus"
# RUN dotnet build "Webstatus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore "Webstatus.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Webstatus.dll"]