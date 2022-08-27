#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["src/API/BoardGameWebService/BoardGameWebService.csproj", "src/API/BoardGameWebService/"]
RUN dotnet restore "src/API/BoardGameWebService/BoardGameWebService.csproj"
COPY . .
WORKDIR "/src/src/API/BoardGameWebService"

FROM build AS publish
RUN dotnet publish "BoardGameWebService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BoardGameWebService.dll"]