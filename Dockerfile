FROM mcr.microsoft.com/dotnet/sdk:5.0.100-rc.1-focal AS publish
# Cache NuGet packages
WORKDIR /src
COPY ["BlazorEventsTodo/Shared/BlazorEventsTodo.Shared.csproj", "BlazorEventsTodo/Shared/"]
COPY ["BlazorEventsTodo/Server/BlazorEventsTodo.Server.csproj", "BlazorEventsTodo/Server/"]
COPY ["BlazorEventsTodo/Client/BlazorEventsTodo.Client.csproj", "BlazorEventsTodo/Client/"]
COPY ["BlazorEventsTodo/BlazorEventsTodo.Test/BlazorEventsTodo.Test.csproj", "BlazorEventsTodo/BlazorEventsTodo.Test/"]
COPY ["BlazorEventsTodo.sln", ""]
RUN dotnet restore "BlazorEventsTodo.sln"

# copy remaining application files
COPY . .

# release app
WORKDIR /src
RUN dotnet publish "BlazorEventsTodo.sln" -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0.0-rc.1-focal AS final

WORKDIR /app

EXPOSE 80
EXPOSE 443

COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BlazorEventsTodo.Server.dll"]