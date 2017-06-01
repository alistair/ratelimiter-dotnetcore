FROM microsoft/dotnet:1.1.1-sdk

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

COPY . /usr/src/app

ADD ./docker/build/build.sh /usr/src/app/build.sh
ADD ./docker/functionaltests/run.sh /usr/src/app/functionaltest.sh

ENV USER_ID 1000
ENV GROUP_ID 1000
