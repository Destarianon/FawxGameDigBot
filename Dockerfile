#create base container
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
#install NodeJS
RUN apt-get update && apt-get install -y curl ca-certificates gnupg git
RUN curl -fsSL https://deb.nodesource.com/setup_lts.x | bash - &&\
	apt-get install -y nodejs
#RUN npm install -g gamedig@^5.0.0-beta.0
#build node-gamedig from source
WORKDIR /gamedig
RUN git clone https://github.com/gamedig/node-gamedig . &&\
    npm install
RUN npm link
WORKDIR /app
USER $APP_UID

#build layer
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FawxGameDigBot.csproj", "./"]
RUN dotnet restore "FawxGameDigBot.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "FawxGameDigBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

#publish layer
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FawxGameDigBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

#finalize container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FawxGameDigBot.dll"]
