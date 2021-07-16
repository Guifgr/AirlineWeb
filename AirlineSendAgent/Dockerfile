FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AirlineSendAgent/AirlineSendAgent.csproj", "AirlineSendAgent/"]
RUN dotnet restore "AirlineSendAgent/AirlineSendAgent.csproj"
COPY . .
WORKDIR "/src/AirlineSendAgent"
RUN dotnet build "AirlineSendAgent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AirlineSendAgent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AirlineSendAgent.dll"]
