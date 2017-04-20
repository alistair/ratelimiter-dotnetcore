FROM microsoft/dotnet:1.1.1-sdk

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

COPY dist/ /usr/src/app

ENV USER_ID 1000
ENV GROUP_ID 1000

EXPOSE 5000

ENTRYPOINT [ "dotnet", "ratelimiter.dll" ]
