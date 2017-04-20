#!/bin/bash

docker-compose -f docker/config/app.yml -f docker/config/redis.yml -f docker/config/network.yml build
docker-compose -f docker/config/app.yml -f docker/config/redis.yml -f docker/config/network.yml up
