FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HomeApi.csproj", "./"]
RUN dotnet restore "HomeApi.csproj"
COPY . .
RUN dotnet build "HomeApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8082
ENTRYPOINT ["dotnet", "HomeApi.dll"]
