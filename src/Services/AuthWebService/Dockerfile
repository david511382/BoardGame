#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY src/Repositories/Member/MemberRepository.csproj  ./Repositories/Member/
COPY src/Services/AuthWebService/AuthWebService.csproj  ./Services/AuthWebService/
COPY src/Common/Domain/Domain.csproj ./Common/Domain/
COPY src/Common/Util/Util.csproj ./Common/Util/

RUN dotnet restore  ./Services/AuthWebService/AuthWebService.csproj

COPY src/Repositories/Member/. ./Repositories/Member/
COPY src/Services/AuthWebService/. ./Services/AuthWebService/
COPY src/Common/Domain/. ./Common/Domain/
COPY src/Common/Util/. ./Common/Util/
WORKDIR /src/Services/AuthWebService
RUN dotnet publish -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AuthWebService.dll"]