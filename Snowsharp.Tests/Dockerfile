FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /tmp

RUN apt-get update && apt-get install -y \
    ca-certificates \
    wget \
    unzip

# COPY ./ca-bundle.crt /usr/local/share/ca-certificates/
# RUN update-ca-certificates

WORKDIR /src
COPY ["SnowplowTests/SnowplowTests.csproj", "SnowplowTests/"]
COPY ["Snowplow/Snowplow.csproj", "Snowplow/"]

RUN dotnet restore "SnowplowTests/SnowplowTests.csproj"

COPY . .

### Run all tests ###
CMD ["dotnet", "test"]
