# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Zwervfafval_WebApp/Zwervfafval_WebApp.csproj .
RUN dotnet restore
COPY Zwervfafval_WebApp/ .
RUN dotnet publish -c Release -o /app/out

# runtime
FROM nginx:alpine
COPY --from=build /app/out/wwwroot /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
