FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# copy csproj and restore
COPY LittleFlowerBot/*.csproj ./LittleFlowerBot/
RUN dotnet restore LittleFlowerBot/LittleFlowerBot.csproj

# copy everything else and build app
COPY LittleFlowerBot/ ./LittleFlowerBot/
WORKDIR /app/LittleFlowerBot
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/LittleFlowerBot/out ./

EXPOSE 10000

CMD ["dotnet", "LittleFlowerBot.dll"]
