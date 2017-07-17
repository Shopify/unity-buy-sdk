#!/bin/bash

. "$(dirname $0)/common.sh"

UNITY_LICENSE_LOG_PATH="$PROJECT_ROOT"/license.log
UNITY_LICENSE_PATH='/Library/Application Support/Unity/'

which "$UNITY_PATH" &> /dev/null || die "Unity does not exist at $UNITY_PATH"

# Activate license
if [[ -n $UNITY_USERNAME && -n $UNITY_PASSWORD && -n $UNITY_SERIAL  ]] ; then
    printf 'Unity acquiring %s license ...\n' "$UNITY_USERNAME"

    "$UNITY_PATH" \
        -quit \
        -batchmode \
        -serial "$UNITY_SERIAL" \
        -username "$UNITY_USERNAME" \
        -password "$UNITY_PASSWORD" \
        -logFile "$UNITY_LICENSE_LOG_PATH"

    ACTIVATE_SUCCESS=$?
    LICENSE_EXIST=$(ls -A "$UNITY_LICENSE_PATH")

    if [[ $ACTIVATE_SUCCESS = 0 && $LICENSE_EXIST ]] ; then
        printf 'Acquired license\n'
    else
        printf '########## Failed to acquire license ##########\n'
        cat "$UNITY_LICENSE_LOG_PATH"
        exit 1
    fi
else
    printf 'Unity building with default license'
fi

exit 0
