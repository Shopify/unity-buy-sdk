#!/bin/bash

. $(dirname $0)/common.sh

UNITY_LICENSE_LOG_PATH="$PROJECT_ROOT"/license.log

which "$UNITY_PATH" &> /dev/null || die "Unity does not exist at $UNITY_PATH"

# Deactivate license
if [[ -n "$UNITY_USERNAME" && -n "$UNITY_PASSWORD" && -n "$UNITY_SERIAL"  ]] ; then
    echo "Deactivate license"

    "$UNITY_PATH" \
        -quit \
        -batchmode \
        -nographics \
        -returnlicense

    if [ $? = 0 ] ; then
        echo "Deactivated license"
        exit 0
    else
        echo "Failed to deactivate license"
        echo "------------------\n\n"
        exit 1
    fi
fi

exit 0
