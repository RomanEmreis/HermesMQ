FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

ARG DEPLOYMENT_PORT
EXPOSE $DEPLOYMENT_PORT

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Hermes.MessageQueue.Service/Hermes.MessageQueue.Service.csproj", "src/Hermes.MessageQueue.Service/"]
RUN dotnet restore "src/Hermes.MessageQueue.Service/Hermes.MessageQueue.Service.csproj"
COPY . .
WORKDIR "/src/src/Hermes.MessageQueue.Service"
RUN dotnet build "Hermes.MessageQueue.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hermes.MessageQueue.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hermes.MessageQueue.Service.dll"]