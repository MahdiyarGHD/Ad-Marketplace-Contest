FROM mcr.hamdocker.ir/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.hamdocker.ir/dotnet/sdk:10.0  AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY nuget.config .
COPY ["AdMarketplace/AdMarketplace.csproj", "AdMarketplace/"]
COPY ["AdMarketplace.Infra/AdMarketplace.Infra.csproj", "AdMarketplace.Infra/"]
COPY ["AdMarketplace.Database/AdMarketplace.Database.csproj", "AdMarketplace.Database/"]
COPY ["AdMarketplace.Domain/AdMarketplace.Domain.csproj", "AdMarketplace.Domain/"]
RUN dotnet restore "AdMarketplace/AdMarketplace.csproj"
COPY . .
WORKDIR "/src/AdMarketplace"
RUN dotnet build "./AdMarketplace.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AdMarketplace.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdMarketplace.dll"]
