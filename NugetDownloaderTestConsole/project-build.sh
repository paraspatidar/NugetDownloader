﻿#!/bin/sh
dotnet restore netCoreConsoleApp.csproj
dotnet publish netCoreConsoleApp.csproj -c Release -o out