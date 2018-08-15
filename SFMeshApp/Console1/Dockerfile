FROM yizhang82/aspnetcore-2.0-windowsservercore-2125 AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY Console1/packages ./
COPY Console1/nuget.Config ./
COPY Console1/Console1.csproj Console1/
RUN dotnet restore Console1/Console1.csproj
COPY . .
WORKDIR /src/Console1
RUN dotnet build Console1.csproj -c Release -o /app

#ADD https://download.microsoft.com/download/6/A/A/6AA4EDFF-645B-48C5-81CC-ED5963AEAD48/vc_redist.x64.exe /vc_redist.x64.exe
#RUN vcredist_x64.exe  /install /quiet
 

FROM build AS publish
RUN dotnet publish Console1.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Console1.dll"]