#! /bin/sh

export HOME=/root
cd /home/nick/HomeMonitor/src/MiosMonitor
exec dotnet  -v run >> mios.txt

