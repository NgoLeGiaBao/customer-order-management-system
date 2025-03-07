FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy toàn bộ mã nguồn
COPY . ./

# Build ứng dụng
RUN dotnet build AuthService.csproj -c Release -o /app/build

# Publish ứng dụng
RUN dotnet publish AuthService.csproj -c Release -o /app/publish

# Tạo image final từ ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80

# Copy từ build sang final
COPY --from=build /app/publish ./ 

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "AuthService.dll"]
