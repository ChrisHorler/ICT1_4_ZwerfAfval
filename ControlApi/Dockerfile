# ------- Build Stage -------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ControlApi/ControlApi.csproj ControlApi/
RUN dotnet restore "ControlApi/ControlApi.csproj"

COPY . .
WORKDIR /src/ControlApi
RUN dotnet publish ControlApi.csproj -c Release -o /app/out

# ------- Runtime -------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ControlApi.dll"]