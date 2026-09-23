FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["BookManager.csproj", "./"]
RUN dotnet restore "BookManager.csproj"

COPY . .
RUN dotnet publish "BookManager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
RUN apt-get update \
	&& apt-get install -y --no-install-recommends libkrb5-3 \
	&& rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookManager.dll"]
