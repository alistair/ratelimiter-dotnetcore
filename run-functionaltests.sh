#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd)"
cd "${DIR}"

RESULT=0

rm -Rf report
mkdir -p "${DIR}/report"

if [[ -z $(which docker-compose) ]]; then
  echo "Can't find docker-compose.  Please install it before trying to re-run this script"
  echo "Go to http://docs.docker.com/compose/install/ for details of how to install docker-compose"
fi

die() {
  if [[ -n "${1}" ]]; then
     echo "${1}"
  fi
  teardown_docker
  exit 1
}

trap die EXIT

start_docker() {
  sleep 10
  docker-compose -f docker/config/redis.yml \
                 -f docker/config/network.yml \
                 -f docker/config/eventstore.yml \
                 -f docker/config/app.yml \
                 -p ratelimiter \
                 up -d
}

teardown_docker() {
  docker-compose -f docker/config/redis.yml \
                 -f docker/config/network.yml \
                 -f docker/config/eventstore.yml \
                 -f docker/config/app.yml \
                 -p ratelimiter \
                 kill
  docker-compose -f docker/config/redis.yml \
                 -f docker/config/network.yml \
                 -f docker/config/eventstore.yml \
                 -f docker/config/app.yml \
                 -p ratelimiter \
                 rm --force
}

run_tests() {
  docker-compose -f docker/config/redis.yml \
                 -f docker/config/network.yml \
                 -f docker/config/eventstore.yml \
                 -f docker/config/app.yml \
                 -f docker/config/functionaltests.yml \
                 -p ratelimiter run functionaltests /usr/src/app/functionaltest.sh
  RESULT="$?"
}

start_docker
run_tests
teardown_docker

exit ${RESULT}
