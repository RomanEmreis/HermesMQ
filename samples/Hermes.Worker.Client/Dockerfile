#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["samples/Hermes.Worker.Client/Hermes.Worker.Client.csproj", "samples/Hermes.Worker.Client/"]
RUN dotnet restore "samples/Hermes.Worker.Client/Hermes.Worker.Client.csproj"
COPY . .
WORKDIR "/src/samples/Hermes.Worker.Client"
RUN dotnet build "Hermes.Worker.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hermes.Worker.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hermes.Worker.Client.dll"]