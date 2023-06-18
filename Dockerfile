FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["UserManagement.Api/*.csproj", "./UserManagement.Api/"]
COPY ["UserManagement.Domain/*.csproj", "./UserManagement.Domain/"]
COPY ["UserManagement.Infrastructure/*.csproj", "./UserManagement.Infrastructure/"]
COPY ["UserManagement.Interfaces/*.csproj", "./UserManagement.Interfaces/"]

RUN dotnet restore "./UserManagement.Interfaces/"
RUN dotnet restore "./UserManagement.Domain/"
RUN dotnet restore "./UserManagement.Infrastructure/"
RUN dotnet restore "./UserManagement.Api/"

COPY . .

WORKDIR "/src/UserManagement.Interfaces"
RUN dotnet build -c Release -o /app
WORKDIR "/src/UserManagement.Domain"
RUN dotnet build -c Release -o /app
WORKDIR "/src/UserManagement.Infrastructure"
RUN dotnet build -c Release -o /app
WORKDIR "/src/UserManagement.Api"
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish  -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManagement.Api.dll"]