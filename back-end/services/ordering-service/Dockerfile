FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY OrderingService.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 83

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "OrderingService.dll"]
