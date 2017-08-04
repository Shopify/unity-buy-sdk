#!/bin/bash

. "$(dirname "$0")"/common.sh
. "$(dirname "$0")"/has_license_credentials.sh

UNITY_LICENSE_LOG_PATH="$PROJECT_ROOT"/license.log
UNITY_LICENSE_PATH='/Library/Application Support/Unity/'

which "$UNITY_PATH" &> /dev/null || die "Unity does not exist at $UNITY_PATH"

has_license_credentials || {
    printf "Unity building with default license\n"
    exit 0
}

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
    exit 0
else
    printf '########## Failed to acquire license ##########\n'
    cat "$UNITY_LICENSE_LOG_PATH"
    exit 1
fi
