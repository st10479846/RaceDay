# 1. Runtime Image (used to actually run the website)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# 2. Build Image (used to compile the code)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project files first for NuGet restore
COPY ["RaceDay/RaceDay.Mvc/RaceDay.Mvc.csproj", "RaceDay.Mvc/"]
COPY ["RaceDay/RaceDay.Contracts/RaceDay.Contracts.csproj", "RaceDay.Contracts/"]

# Restore packages
RUN dotnet restore "RaceDay.Mvc/RaceDay.Mvc.csproj"

# Copy the REST of the RaceDay folder (Program.cs, Views, etc.)
COPY RaceDay/. .

# Set the working directory and build
WORKDIR "/src/RaceDay.Mvc"
RUN dotnet build "RaceDay.Mvc.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Publish Image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RaceDay.Mvc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Final Image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RaceDay.Mvc.dll"]