#!/bin/bash

. "$(dirname $0)/common.sh"

UNITY_LICENSE_LOG_PATH="$PROJECT_ROOT"/deactivate_license.log

which "$UNITY_PATH" &> /dev/null || die "Unity does not exist at $UNITY_PATH"

# Deactivate license
if [[ -n "$UNITY_USERNAME" && -n "$UNITY_PASSWORD" && -n "$UNITY_SERIAL"  ]] ; then
    printf 'Deactivating license ... \n'

    "$UNITY_PATH" \
        -quit \
        -batchmode \
        -returnlicense \
        -logFile "$UNITY_LICENSE_LOG_PATH"

    DEACTIVATE_SUCCESS=$?

    if [[ $DEACTIVATE_SUCCESS = 0 ]] ; then
        printf 'Deactivated license\n'
    else
        printf '########## Failed to deactivate license ##########\n'
        cat "$UNITY_LICENSE_LOG_PATH"
        exit 1
    fi
fi

exit 0
