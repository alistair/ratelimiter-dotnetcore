#!/bin/bash

docker-compose -f docker/config/redis.yml \
               -f docker/config/network.yml \
               -f docker/config/eventstore.yml \
               build

docker-compose -f docker/config/redis.yml \
               -f docker/config/network.yml \
               -f docker/config/eventstore.yml \
               up
