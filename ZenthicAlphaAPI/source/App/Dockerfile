# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /source/app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["source/App/App.csproj", "source/App/"]
COPY ["source/core/Domain/Domain.csproj", "source/Domain/"]
COPY ["source/core/Application/Application.csproj", "source/Application/"]
COPY ["source/core/Infrastructure/Infrastructure.csproj", "source/Infrastructure/"]
COPY ["source/core/Presentation/Presentation.csproj", "source/Presentation/"]
COPY ["source/modules/identity/Identity.Domain/Identity.Domain.csproj", "source/modules/identity/Identity.Domain/"]
COPY ["source/modules/identity/Identity.Application/Identity.Application.csproj", "source/modules/identity/Identity.Application/"]
COPY ["source/modules/identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "source/modules/identity/Identity.Infrastructure/"]
COPY ["source/modules/identity/Identity.Presentation/Identity.Presentation.csproj", "source/modules/identity/Identity.Presentation/"]
RUN dotnet restore "./source/App/App.csproj"
COPY . .
WORKDIR "/src/source/App"
RUN dotnet build "./App.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.dll"]