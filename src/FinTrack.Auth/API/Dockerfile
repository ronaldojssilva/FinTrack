FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FinTrack.Auth/Domain/FinTrack.Auth.Domain.csproj",           "FinTrack.Auth/Domain/"]
COPY ["FinTrack.Auth/Application/FinTrack.Auth.Application.csproj", "FinTrack.Auth/Application/"]
COPY ["FinTrack.Auth/Infrastructure/FinTrack.Auth.Infrastructure.csproj", "FinTrack.Auth/Infrastructure/"]
COPY ["FinTrack.Auth/API/FinTrack.Auth.API.csproj",                 "FinTrack.Auth/API/"]
COPY ["FinTrack.Shared/FinTrack.Shared.csproj",                     "FinTrack.Shared/"]

RUN dotnet restore "FinTrack.Auth/API/FinTrack.Auth.API.csproj"

COPY . .
RUN dotnet publish "FinTrack.Auth/API/FinTrack.Auth.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinTrack.Auth.API.dll"]
