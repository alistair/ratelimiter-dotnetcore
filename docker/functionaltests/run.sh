#!/bin/bash

set -e

function cleanup() {
  echo "cleanup"
  chown $USER_ID:$GROUP_ID /usr/src/app/report/report.html
}

trap cleanup EXIT

cd src/functionaltests/
dotnet restore
dotnet storyteller run -r /usr/src/app/report/report.html
