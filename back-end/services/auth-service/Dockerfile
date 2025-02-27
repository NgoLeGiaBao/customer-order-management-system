# Sử dụng image base cho .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Đặt thư mục làm việc
WORKDIR /src

# Copy file .csproj và restore các dependency
COPY AuthService.csproj ./
RUN dotnet restore

# Copy phần còn lại của mã nguồn
COPY . ./

# Build ứng dụng
RUN dotnet build -c Release -o /app/build

# Publish ứng dụng
RUN dotnet publish -c Release -o /app/publish

# Xây dựng image final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80

# Copy từ build sang final
COPY --from=build /app/publish ./

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "AuthService.dll"]
