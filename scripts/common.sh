#!/bin/bash

die() {
    out "$*"
    exit 1
}

out() {
    echo -e $*
}

start() {
    out "Starting: $*"
}

end() {
    out "Finished: $*"
}

PROJECT_ROOT=${PROJECT_ROOT:-$(pwd)}
SCRIPTS_ROOT="$PROJECT_ROOT/scripts"
# UNITY_APP_PATH="${UNITY_APP_PATH:-/Applications/Unity/Hub/Editor/2019.4.18f1/Unity.app}"
UNITY_APP_PATH="${UNITY_APP_PATH:-/Applications/Unity/Unity.app}"
UNITY_PATH="$UNITY_APP_PATH/Contents/MacOS/Unity"
UNITY_PACKAGE_MANAGER_PATH="$UNITY_APP_PATH/Contents/PackageManager/Unity/PackageManager"
