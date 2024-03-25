#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Bookery.User/Bookery.User.csproj", "Bookery.User/"]
RUN dotnet restore "Bookery.User/Bookery.User.csproj"
COPY . .
WORKDIR "/src/Bookery.User"
RUN dotnet build "Bookery.User.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bookery.User.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookery.User.dll"]
