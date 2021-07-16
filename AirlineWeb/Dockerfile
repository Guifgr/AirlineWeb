FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AirlineWeb/AirlineWeb.csproj", "AirlineWeb/"]
RUN dotnet restore "AirlineWeb/AirlineWeb.csproj"
COPY . .
WORKDIR "/src/AirlineWeb"
RUN dotnet build "AirlineWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AirlineWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AirlineWeb.dll"]
