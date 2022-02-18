#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Bookery.Api/Bookery.Api.csproj", "Bookery.Api/"]
COPY ["Bookery.Config/Bookery.Config.csproj", "Bookery.Config/"]
RUN dotnet restore "Bookery.Api/Bookery.Api.csproj"
COPY . .
WORKDIR "/src/Bookery.Api"
RUN dotnet build "Bookery.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bookery.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookery.Api.dll"]