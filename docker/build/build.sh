#!/bin/bash

set -e

function cleanup {
  chown -R $USER_ID:$GROUP_ID dist
}

trap cleanup EXIT

dotnet restore src/
dotnet build src/

echo "Running publish"
dotnet publish -f netcoreapp1.1 -c Release src/ratelimiter/

ls src/ratelimiter/bin/Release/netcoreapp1.1/publish/
cp -R src/ratelimiter/bin/Release/netcoreapp1.1/publish/* dist/
