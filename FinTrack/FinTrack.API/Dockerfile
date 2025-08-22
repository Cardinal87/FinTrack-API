# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FinTrack.API/FinTrack.API.csproj", "FinTrack.API/"]
COPY ["FinTrack.API.Application/FinTrack.API.Application.csproj", "FinTrack.API.Application/"]
COPY ["FinTrack.API.Core/FinTrack.API.Core.csproj", "FinTrack.API.Core/"]
COPY ["FinTrack.API.Infrastructure/FinTrack.API.Infrastructure.csproj", "FinTrack.API.Infrastructure/"]
RUN dotnet restore "./FinTrack.API/FinTrack.API.csproj"
COPY . .
WORKDIR "/src/FinTrack.API"
RUN dotnet build "./FinTrack.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinTrack.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
RUN mkdir -p /app/logs
RUN chmod 777 /app/logs
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinTrack.API.dll"]