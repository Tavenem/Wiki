FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

ARG PAT
RUN wget -qO- https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | bash
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
ENV NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED true
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS {\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/wilstead/_packaging/WS-NuGet/nuget/v3/index.json\", \"username\":\"Docker nuget\", \"password\":\"${PAT}\"}]}

WORKDIR /src
COPY ["NeverFoundry.Wiki.Samples.Complete.csproj", "NeverFoundry.Wiki.Samples.Complete/"]
RUN dotnet restore "NeverFoundry.Wiki.Samples.Complete/NeverFoundry.Wiki.Samples.Complete.csproj" -s "https://api.nuget.org/v3/index.json" -s "https://pkgs.dev.azure.com/wilstead/_packaging/WS-NuGet/nuget/v3/index.json"
COPY . "NeverFoundry.Wiki.Samples.Complete/"
WORKDIR "/src/NeverFoundry.Wiki.Samples.Complete"
RUN dotnet build "NeverFoundry.Wiki.Samples.Complete.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NeverFoundry.Wiki.Samples.Complete.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NeverFoundry.Wiki.Samples.Complete.dll"]