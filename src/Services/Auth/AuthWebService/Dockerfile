#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY src/Repositories/Member/MemberRepository.csproj  ./Repositories/Member/
COPY src/Services/Auth/AuthWebService/AuthWebService.csproj  ./Services/Auth/AuthWebService/
COPY src/Common/Domain/Domain.csproj ./Common/Domain/
COPY src/Common/Util/CommonUtil/Convert.csproj ./Common/Util/CommonUtil/

RUN dotnet restore  ./Services/Auth/AuthWebService/AuthWebService.csproj

COPY src/Repositories/Member/. ./Repositories/Member/
COPY src/Services/Auth/AuthWebService/. ./Services/Auth/AuthWebService/
COPY src/Common/Domain/. ./Common/Domain/
COPY src/Common/Util/CommonUtil/. ./Common/Util/CommonUtil/
WORKDIR /src/Services/Auth/AuthWebService
RUN dotnet publish -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AuthWebService.dll"]