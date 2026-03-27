#!/bin/bash

add-apt-repository -y ppa:dotnet/backports
apt-get update

apt-get install -y aspnetcore-runtime-8.0
apt-get install -y aspnetcore-runtime-9.0
apt-get install -y dotnet-sdk-10.0

# Install .NET 11 preview SDK (not yet available in apt)
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 11.0 --quality preview --install-dir /usr/share/dotnet
