. $(dirname $0)/common.sh

# set UNITY_DOWNLOAD_DIR if it hasn't been set. It may have been set from
UNITY_DOWNLOAD_DIR="${UNITY_DOWNLOAD_DIR:-`pwd`/unity}"
UNITY_PKG_LOCATION=${UNITY_PKG_LOCATION:-$UNITY_DOWNLOAD_DIR/Unity.pkg}
UNITY_PKG_URL=${UNITY_PKG_URL:-http://netstorage.unity3d.com/unity/3829d7f588f3/MacEditorInstaller/Unity-5.5.2f1.pkg}
IOS_PKG_LOCATION=${IOS_PKG_LOCATION:-$UNITY_DOWNLOAD_DIR/Unity-iOS.pkg}
IOS_PKG_URL=${IOS_PKG_URL:-http://netstorage.unity3d.com/unity/3829d7f588f3/MacEditorTargetInstaller/UnitySetup-iOS-Support-for-Editor-5.5.2f1.pkg}

rm -f "$UNITY_PKG_LOCATION"
if [ ! -e "$UNITY_PKG_LOCATION" ]; then
    out "Downloading Unity to $UNITY_DOWNLOAD_DIR"
    out "Downloading from {$UNITY_PKG_URL}"
    mkdir -p $UNITY_DOWNLOAD_DIR
    curl -o $UNITY_PKG_LOCATION $UNITY_PKG_URL
    out "Finished Downloading Unity"
else
    out "$UNITY_PKG_LOCATION already exists"
    ls /Users/shopify/unity-buy-sdk/unity-buy-sdk/unity
fi

if [ ! -e "$IOS_PKG_LOCATION" ]; then
    out "Downloading iOS Editor Support to $UNITY_DOWNLOAD_DIR"
    out "Downloading from {$IOS_PKG_URL}"
    curl -o $IOS_PKG_LOCATION $IOS_PKG_URL
    out "Finished Downloading iOS Editor Support"
fi

out "Start Install Unity"
sudo installer -dumplog -package $UNITY_PKG_LOCATION -target /

if [ $? = 0 ] ; then
    out "Finished Install Unity"
else
    die "Unable to install Unity"
fi

out "Start Install iOS Editor Support"
sudo installer -dumplog -package $IOS_PKG_LOCATION -target /

if [ $? = 0 ] ; then
    out "Finished Install iOS Editor Support"
else
    die "Unable to install iOS Editor Support"
fi
