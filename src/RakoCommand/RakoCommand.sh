#! /bin/sh

if [ "$HOME" = "" ]
then
  export HOME=/root
fi
cd /home/nick/HomeMonitor/src/RakoCommand
dotnet  run $*

