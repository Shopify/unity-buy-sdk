#!/bin/bash

. "$(dirname "$0")"/common.sh

# set UNITY_DOWNLOAD_DIR if it hasn't been set. It may have been set from 
UNITY_DOWNLOAD_DIR="${UNITY_DOWNLOAD_DIR:-`pwd`/unity}"
UNITY_PKG_LOCATION=${UNITY_PKG_LOCATION:-"$UNITY_DOWNLOAD_DIR"/Unity.pkg}
UNITY_PKG_URL=${UNITY_PKG_URL:-https://download.unity3d.com/download_unity/5d30cf096e79/MacEditorInstaller/Unity-2017.1.1f1.pkg}

if [[ ! -e $UNITY_PKG_LOCATION ]] ; then
    out "Downloading Unity to $UNITY_DOWNLOAD_DIR"
    out "Downloading from {$UNITY_PKG_URL}"
    mkdir -p "$UNITY_DOWNLOAD_DIR"
    curl -o "$UNITY_PKG_LOCATION" "$UNITY_PKG_URL"
    out "Finished Downloading Unity"
fi

out "Start Install Unity"
sudo installer -dumplog -package "$UNITY_PKG_LOCATION" -target /
INSTALL_UNITY_RESULT=$?

if [[ $INSTALL_UNITY_RESULT = 0 ]] ; then
    out "Finished Install Unity"
else
    die "Unable to install Unity"
fi
