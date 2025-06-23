FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Livena.Identity.Api/Livena.Identity.Api.csproj", "src/Livena.Identity.Api/"]
RUN dotnet restore "src/Livena.Identity.Api/Livena.Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/Livena.Identity.Api"
RUN dotnet build "Livena.Identity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Livena.Identity.Api.dll"]
