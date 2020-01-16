#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY src/Repositories/Game/GameRespository.csproj  ./Repositories/Game/
COPY src/Repositories/Redis/RedisRepository.csproj  ./Repositories/Redis/
COPY src/Services/Game/GameWebService/GameWebService.csproj  ./Services/Game/GameWebService/
COPY src/Services/Game/BigTwoLogic/BigTwoLogic.csproj  ./Services/Game/BigTwoLogic/
COPY src/Services/Game/GameLogic/GameLogic.csproj  ./Services/Game/GameLogic/
COPY src/Common/Domain/Domain.csproj ./Common/Domain/
COPY src/Common/Util/CommonUtil/Convert.csproj ./Common/Util/CommonUtil/

RUN dotnet restore  ./Services/Game/GameWebService/GameWebService.csproj

COPY src/Repositories/Game/. ./Repositories/Game/
COPY src/Repositories/Redis/. ./Repositories/Redis/
COPY src/Services/Game/GameWebService/. ./Services/Game/GameWebService/
COPY src/Services/Game/BigTwoLogic/. ./Services/Game/BigTwoLogic/
COPY src/Services/Game/GameLogic/. ./Services/Game/GameLogic/
COPY src/Common/Domain/. ./Common/Domain/
COPY src/Common/Util/CommonUtil/. ./Common/Util/CommonUtil/
WORKDIR /src/Services/Game/GameWebService
RUN dotnet publish -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GameWebService.dll"]