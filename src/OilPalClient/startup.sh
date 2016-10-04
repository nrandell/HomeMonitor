#! /bin/sh

export HOME=/root
cd /home/nick/HomeMonitor/src/OilPalClient
exec dotnet  -v run >> oilpal.txt

