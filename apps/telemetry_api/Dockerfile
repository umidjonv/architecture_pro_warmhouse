FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TelemetryApi.csproj", "./"]
RUN dotnet restore "TelemetryApi.csproj"
COPY . .
RUN dotnet build "TelemetryApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelemetryApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8083
ENTRYPOINT ["dotnet", "TelemetryApi.dll"]
