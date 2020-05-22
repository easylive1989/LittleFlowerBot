FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
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

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime

ARG POSTGRE_SQL_CONN_STR
ARG REDIS_HOST
ARG LINE_CHANNEL_TOKEN
ARG LINE_NOTIFY_CLIENT_ID
ARG LINE_NOTIFY_CLIENT_SECRET
ENV ENV_POSTGRE_SQL_CONN_STR=$POSTGRE_SQL_CONN_STR
ENV ENV_REDIS_HOST=$REDIS_HOST
ENV ENV_LINE_CHANNEL_TOKEN=$LINE_CHANNEL_TOKEN
ENV ENV_LINE_NOTIFY_CLIENT_ID=$LINE_NOTIFY_CLIENT_ID
ENV ENV_LINE_NOTIFY_CLIENT_SECRET=$LINE_NOTIFY_CLIENT_SECRET
WORKDIR /app
COPY --from=build /app/LittleFlowerBot/out ./
CMD ASPNETCORE_URLS=http://*:$PORT dotnet LittleFlowerBot.dll --ConnectionStrings:DefaultConnection="$ENV_POSTGRE_SQL_CONN_STR" --Redis:Host="$ENV_REDIS_HOST" --LineChannelToken="$ENV_LINE_CHANNEL_TOKEN" --LineNotifyClientId="$ENV_LINE_NOTIFY_CLIENT_ID" --LineNotifyClientSecret="$ENV_LINE_NOTIFY_CLIENT_SECRET"
