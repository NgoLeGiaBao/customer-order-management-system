FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy toàn bộ mã nguồn
COPY . ./

# Build ứng dụng
RUN dotnet build menu-service.csproj -c Release -o /app/build

# Publish ứng dụng
RUN dotnet publish menu-service.csproj -c Release -o /app/publish

# Tạo image final từ ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 81

# Copy từ build sang final
COPY --from=build /app/publish ./ 

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "menu-service.dll"]
