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

retries=0

while [ $retries -lt 10 ]
do
    printf "Unity acquiring %s license... (UNITY_USERNAME: $UNITY_USERNAME retries: $retries)\n"

    "$UNITY_PATH" \
        -quit \
        -batchmode \
        -serial "$UNITY_SERIAL" \
        -username "$UNITY_USERNAME" \
        -password "$UNITY_PASSWORD" \
        -logFile "$UNITY_LICENSE_LOG_PATH" \
        -projectPath "/Users/travis/build/Shopify/unity-buy-sdk/"

    ACTIVATE_SUCCESS=$?
    LICENSE_EXIST=$(ls -A "$UNITY_LICENSE_PATH")

    if [[ $ACTIVATE_SUCCESS = 0 && $LICENSE_EXIST ]] ; then
        printf 'Acquired license...!\n'
        exit 0
    fi

    sleep_duration=$((($retries + 1) * 60))

    printf "Could not get license, retrying in ${sleep_duration} seconds...\n"

    cat "$UNITY_LICENSE_LOG_PATH"

    sleep $sleep_duration

    retries=$[$retries+1]
done

printf '########## Failed to acquire license ##########\n'
cat "$UNITY_LICENSE_LOG_PATH"
exit 1



