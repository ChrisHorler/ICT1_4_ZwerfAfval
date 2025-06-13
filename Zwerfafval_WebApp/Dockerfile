# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy and Restore
COPY Zwerfafval_WebApp.csproj ./
RUN dotnet restore Zwerfafval_WebApp.csproj

# Copy all project files and publish
COPY . ./
RUN dotnet publish -c Release -o /app/out

# --- Runtime stage ---
FROM nginx:alpine
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build /app/out/wwwroot /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
