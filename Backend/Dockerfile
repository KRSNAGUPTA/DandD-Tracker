# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/DdTracker.Api/DdTracker.Api.csproj", "src/DdTracker.Api/"]
RUN dotnet restore "src/DdTracker.Api/DdTracker.Api.csproj"

COPY . .
WORKDIR "/src/src/DdTracker.Api"
RUN dotnet publish "DdTracker.Api.csproj" -c Release -o /app/publish

#Stage 2: Runtime 
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "DdTracker.Api.dll"]