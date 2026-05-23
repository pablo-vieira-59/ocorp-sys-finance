# =========================
# STAGE 1 - Build Angular
# =========================
FROM node:22 AS angular-build

WORKDIR /app

COPY sys-finance-web/package*.json ./

RUN npm install

COPY sys-finance-web/ .

RUN npm run build -- --configuration production


# =========================
# STAGE 2 - Build .NET API
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build

WORKDIR /src

COPY SysFinance.API/SysFinance.API.csproj SysFinance.API/

RUN dotnet restore SysFinance.API/SysFinance.API.csproj

COPY . .

# Copia build do Angular para wwwroot
COPY --from=angular-build /app/dist/sys-finance-web/browser ./SysFinance.API/wwwroot

WORKDIR /src/SysFinance.API

RUN dotnet publish -c Release -o /app/publish


# =========================
# STAGE 3 - Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=dotnet-build /app/publish .

EXPOSE 8081

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8081

ENTRYPOINT ["dotnet", "SysFinance.API.dll"]