FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["src/Consumer/Consumer.csproj", "Consumer/"]
COPY ["src/Events/Events.csproj", "Events/"]
RUN dotnet restore "Consumer/Consumer.csproj"
COPY ["src/Consumer/.", "Consumer/"]
COPY ["src/Events/.", "Events/"]
WORKDIR "/src/Consumer"

FROM build AS publish
RUN dotnet publish "Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.dll"]
