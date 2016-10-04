#! /bin/sh

export HOME=/root
cd /home/nick/HomeMonitor/src/RakoClient
exec dotnet  -v run >> rako.txt

