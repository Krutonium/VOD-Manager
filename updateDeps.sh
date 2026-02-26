#/usr/bin/env bash
nix run nixpkgs#dotnet-sdk_10 -- restore
nix run nixpkgs#dotnet-sdk_10 -- msbuild -t:GenerateRestoreGraphFile -p:RestoreGraphOutputPath=deps.json
