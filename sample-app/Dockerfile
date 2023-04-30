FROM mcr.microsoft.com/dotnet/sdk:7.0.202 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /app/out/ ./
ENTRYPOINT ["dotnet", "sample-app.dll"]
