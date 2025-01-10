#!/bin/sh

cd "$(dirname "$0")"

TargetOS=Linux ./publish_linux.sh
TargetOS=MacOS ./publish_osx.sh
TargetOS=Windows ./publish_windows.sh
