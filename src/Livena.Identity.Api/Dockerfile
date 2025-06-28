FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Livena.Identity.Api/Livena.Identity.Api.csproj", "Livena.Identity.Api/"]
RUN dotnet restore "Livena.Identity.Api/Livena.Identity.Api.csproj"
COPY . .
WORKDIR "/src/Livena.Identity.Api"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Livena.Identity.Api.dll"]
