#! /bin/sh

export HOME=/root
cd /home/nick/HomeMonitor/src/OwlClient
exec dotnet  -v run >> owl.txt

