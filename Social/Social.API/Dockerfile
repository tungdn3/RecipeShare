#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Social/Social.API/Social.API.csproj", "Social/Social.API/"]
COPY ["Social/Social.Core/Social.Core.csproj", "Social/Social.Core/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Social/Social.Infrastructure/Social.Infrastructure.csproj", "Social/Social.Infrastructure/"]
RUN dotnet restore "./Social/Social.API/./Social.API.csproj"
COPY . .
WORKDIR "/src/Social/Social.API"
RUN dotnet build "./Social.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Social.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Social.API.dll"]