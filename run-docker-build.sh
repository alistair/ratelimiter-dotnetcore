#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd)"
cd "${DIR}"

rm -Rf dist
mkdir -p "${DIR}/dist"

docker build -t ratelimiter/build -f ./docker/build/Dockerfile .
docker run -t -a STDOUT --rm \
       -v "$(pwd)/dist:/usr/src/app/dist" \
       -e USER_ID=$(id -u) \
       -e GROUP_ID=$(id -g) \
       -e BUILD_ID=${BUILD_ID} \
       ratelimiter/build /bin/bash build.sh
