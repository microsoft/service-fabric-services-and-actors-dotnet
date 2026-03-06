#!/bin/bash

add-apt-repository -y ppa:dotnet/backports
apt-get update

apt-get install -y aspnetcore-runtime-8.0
apt-get install -y aspnetcore-runtime-9.0
apt-get install -y dotnet-sdk-10.0
