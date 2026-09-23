FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY ["BookManager.slnx", "./"]
COPY ["src/BookManager/BookManager.API/BookManager.API.csproj", "src/BookManager/BookManager.API/"]
COPY ["src/BookManager/BookManager.Application/BookManager.Application.csproj", "src/BookManager/BookManager.Application/"]
COPY ["src/BookManager/BookManager.Domain/BookManager.Domain.csproj", "src/BookManager/BookManager.Domain/"]
COPY ["src/BookManager/BookManager.Infrastructure/BookManager.Infrastructure.csproj", "src/BookManager/BookManager.Infrastructure/"]

RUN dotnet restore "src/BookManager/BookManager.API/BookManager.API.csproj"

COPY . .
RUN dotnet publish "src/BookManager/BookManager.API/BookManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
RUN apt-get update \
	&& apt-get install -y --no-install-recommends libkrb5-3 \
	&& rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookManager.API.dll"]
