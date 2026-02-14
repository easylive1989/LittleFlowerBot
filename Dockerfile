FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY LittleFlowerBot/*.csproj ./LittleFlowerBot/
COPY LittleFlowerBotTests/*.csproj ./LittleFlowerBotTests/
RUN dotnet restore

# copy everything else and build app
COPY . ./
WORKDIR /app/LittleFlowerBot
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/LittleFlowerBot/out ./

EXPOSE 10000

CMD ["dotnet", "LittleFlowerBot.dll"]
