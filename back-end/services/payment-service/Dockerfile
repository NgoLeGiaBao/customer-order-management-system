FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY PaymentService.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 84

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "PaymentService.dll"]