#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Bookery.Node/Bookery.Node.csproj", "Bookery.Node/"]
RUN dotnet restore "Bookery.Node/Bookery.Node.csproj"
COPY . .
WORKDIR "/src/Bookery.Node"
RUN dotnet build "Bookery.Node.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bookery.Node.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookery.Node.dll"]
