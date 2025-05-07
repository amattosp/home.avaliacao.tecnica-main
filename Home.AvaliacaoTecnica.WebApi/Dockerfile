#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Home.AvaliacaoTecnica.WebApi/Home.AvaliacaoTecnica.WebApi.csproj", "Home.AvaliacaoTecnica.WebApi/"]
COPY ["Home.AvaliacaoTecnica.Contracts/Home.AvaliacaoTecnica.Contracts.csproj", "Home.AvaliacaoTecnica.Contracts/"]
COPY ["Home.AvaliacaoTecnica.Infra.Data/Home.AvaliacaoTecnica.Infra.Data.csproj", "Home.AvaliacaoTecnica.Infra.Data/"]

RUN dotnet restore "./Home.AvaliacaoTecnica.WebApi/Home.AvaliacaoTecnica.WebApi.csproj"
COPY . .
WORKDIR "/src/Home.AvaliacaoTecnica.WebApi"
RUN dotnet build "./Home.AvaliacaoTecnica.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Home.AvaliacaoTecnica.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Home.AvaliacaoTecnica.WebApi.dll"]