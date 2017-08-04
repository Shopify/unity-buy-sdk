#!/bin/bash

. "$(dirname "$0")"/common.sh
. "$(dirname "$0")"/has_license_credentials.sh

UNITY_LICENSE_LOG_PATH="$PROJECT_ROOT"/deactivate_license.log

which "$UNITY_PATH" &> /dev/null || die "Unity does not exist at $UNITY_PATH"

has_license_credentials || {
    exit 0
}

printf 'Deactivating license ... \n'

"$UNITY_PATH" \
    -quit \
    -batchmode \
    -returnlicense \
    -logFile "$UNITY_LICENSE_LOG_PATH"

DEACTIVATE_SUCCESS=$?

if [[ $DEACTIVATE_SUCCESS = 0 ]] ; then
    printf 'Deactivated license\n'
    exit 0
else
    printf '########## Failed to deactivate license ##########\n'
    cat "$UNITY_LICENSE_LOG_PATH"
    exit 1
fi
