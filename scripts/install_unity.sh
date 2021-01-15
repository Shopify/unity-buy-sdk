#!/bin/bash

. "$(dirname "$0")"/common.sh

# set UNITY_DOWNLOAD_DIR if it hasn't been set. It may have been set from 
UNITY_DOWNLOAD_DIR="${UNITY_DOWNLOAD_DIR:-`pwd`/unity}"
UNITY_PKG_LOCATION=${UNITY_PKG_LOCATION:-"$UNITY_DOWNLOAD_DIR"/Unity.pkg}

# To update this url go to https://unity3d.com/get-unity/download/archive release notes for the specific version
# and get the component link for unity editor
UNITY_PKG_URL=${UNITY_PKG_URL:-https://download.unity3d.com/download_unity/3310a4d4f880/MacEditorInstaller/Unity-2019.4.18f1.pkg}

IOS_PKG_LOCATION=${IOS_PKG_LOCATION:-"$UNITY_DOWNLOAD_DIR"/Unity-iOS.pkg}

# To update this url go to https://unity3d.com/get-unity/download/archive release notes for the specific version
# and get the component link for iOS target support
IOS_PKG_URL=${IOS_PKG_URL:-http://netstorage.unity3d.com/unity/3310a4d4f880/MacEditorTargetInstaller/UnitySetup-iOS-Support-for-Editor.pkg}

if [[ ! -e $UNITY_PKG_LOCATION ]] ; then
    out "Downloading Unity to $UNITY_DOWNLOAD_DIR"
    out "Downloading from {$UNITY_PKG_URL}"
    mkdir -p "$UNITY_DOWNLOAD_DIR"
    curl -o "$UNITY_PKG_LOCATION" "$UNITY_PKG_URL"
    out "Finished Downloading Unity"
fi

if [[ ! -e $IOS_PKG_LOCATION ]] ; then
    out "Downloading iOS Editor Support to $UNITY_DOWNLOAD_DIR"
    out "Downloading from {$IOS_PKG_URL}"
    curl -o "$IOS_PKG_LOCATION" "$IOS_PKG_URL"
    out "Finished Downloading iOS Editor Support"
fi

out "Start Install Unity"
sudo installer -dumplog -package "$UNITY_PKG_LOCATION" -target /
INSTALL_UNITY_RESULT=$?

if [[ $INSTALL_UNITY_RESULT = 0 ]] ; then
    out "Finished Install Unity"
else
    die "Unable to install Unity"
fi

out "Start Install iOS Editor Support"
sudo installer -dumplog -package "$IOS_PKG_LOCATION" -target /
INSTALL_IOS_RESULT=$?

if [[ $INSTALL_IOS_RESULT = 0 ]] ; then
    out "Finished Install iOS Editor Support"
else
    die "Unable to install iOS Editor Support"
fi
