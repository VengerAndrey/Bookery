#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Bookery.Authentication/Bookery.Authentication.csproj", "Bookery.Authentication/"]
RUN dotnet restore "Bookery.Authentication/Bookery.Authentication.csproj"
COPY . .
WORKDIR "/src/Bookery.Authentication"
RUN dotnet build "Bookery.Authentication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bookery.Authentication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookery.Authentication.dll"]