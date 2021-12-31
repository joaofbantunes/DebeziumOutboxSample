FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["src/Producer/Producer.csproj", "Producer/"]
COPY ["src/Events/Events.csproj", "Events/"]
RUN dotnet restore "Producer/Producer.csproj"
COPY ["src/Producer/.", "Producer/"]
COPY ["src/Events/.", "Events/"]
WORKDIR "/src/Producer"

FROM build AS publish
RUN dotnet publish "Producer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Producer.dll"]
