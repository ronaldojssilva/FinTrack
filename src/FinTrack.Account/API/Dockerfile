FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["API/FinTrack.Account.csproj", "API/"]
COPY ["../../FinTrack.Shared/FinTrack.Shared.csproj", "../FinTrack.Shared/"]
RUN dotnet restore "API/FinTrack.Account.csproj"

COPY . .
RUN dotnet build "API/FinTrack.Account.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API/FinTrack.Account.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinTrack.Account.dll"]
