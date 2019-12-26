#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY src/ApiGateways/OcelotApiGateway/OcelotApiGateway.csproj  ./ApiGateways/OcelotApiGateway/
COPY src/Services/Auth/AuthWebService/AuthWebService.csproj  ./Services/Auth/AuthWebService/
COPY src/Services/Auth/MemberRepository/MemberRepository.csproj  ./Services/Auth/MemberRepository/
COPY src/Common/Domain/Domain.csproj ./Common/Domain/
COPY src/Common/Util/CommonUtil/Convert.csproj ./Common/Util/CommonUtil/
COPY src/Common/Util/HttpHelper/HttpHelper.csproj ./Common/Util/HttpHelper/

RUN dotnet restore ./ApiGateways/OcelotApiGateway/OcelotApiGateway.csproj

COPY src/ApiGateways/OcelotApiGateway/. ./ApiGateways/OcelotApiGateway/
COPY src/Services/Auth/AuthWebService/. ./Services/Auth/AuthWebService/
COPY src/Services/Auth/MemberRepository/. ./Services/Auth/MemberRepository/
COPY src/Common/Domain/. ./Common/Domain/
COPY src/Common/Util/CommonUtil/. ./Common/Util/CommonUtil/
COPY src/Common/Util/HttpHelper/. ./Common/Util/HttpHelper/
WORKDIR /src/ApiGateways/OcelotApiGateway/
RUN dotnet publish -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OcelotApiGateway.dll"]