# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala ve restore et
COPY ["StajYerim.API/StajYerim.API.csproj", "StajYerim.API/"]
COPY ["StajYerim.Application/StajYerim.Application.csproj", "StajYerim.Application/"]
COPY ["StajYerim.Core/StajYerim.Core.csproj", "StajYerim.Core/"]
COPY ["StajYerim.Infrastructure/StajYerim.Infrastructure.csproj", "StajYerim.Infrastructure/"]

RUN dotnet restore "StajYerim.API/StajYerim.API.csproj"

# Tüm kodları kopyala ve derle
COPY . .
WORKDIR "/src/StajYerim.API"
RUN dotnet publish "StajYerim.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StajYerim.API.dll"]
