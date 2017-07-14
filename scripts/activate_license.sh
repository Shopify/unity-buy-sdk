#!/bin/bash

. $(dirname $0)/common.sh

UNITY_LICENSE_LOG_PATH="$PROJECT_ROOT"/license.log
UNITY_LICENSE_PATH= "/Library/Application\ Support/Unity/"

which "$UNITY_PATH" &> /dev/null || die "Unity does not exist at $UNITY_PATH"

# Activate license
if [[ -n $UNITY_USERNAME && -n $UNITY_PASSWORD && -n $UNITY_SERIAL  ]] ; then
    echo "Unity building with $UNITY_USERNAME license"

    "$UNITY_PATH" \
        -quit \
        -batchmode \
        -serial "$UNITY_SERIAL" \
        -username "$UNITY_USERNAME" \
        -password "$UNITY_PASSWORD" \
        -logFile "$UNITY_LICENSE_LOG_PATH"

    # Here https://docs.unity3d.com/Manual/CommandLineArguments.html
    # it states that activating the license might take a bit of time :(
    sleep 5
else
    echo "Unity building with default license"
fi

if [[ $? = 0 && $(ls -A $UNITY_LICENSE_PATH) ]] ; then
    echo "Acquired license"
    exit 0
else
    echo "Failed to acquire license"
    echo "------------------\n\n"
    cat "$UNITY_LICENSE_LOG_PATH"
    exit 1
fi
