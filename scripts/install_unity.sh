. $(dirname $0)/common.sh

UNITY_DOWNLOAD_DIR="`pwd`/unity"
UNITY_PKG_LOCATION=$UNITY_DOWNLOAD_DIR/Unity.pkg
UNITY_PKG_URL=http://netstorage.unity3d.com/unity/fdbb5133b820/MacEditorInstaller/Unity-5.3.4f1.pkg

if [ ! -d "$UNITY_DOWNLOAD_DIR" ]; then
    out "Downloading Unity to $UNITY_DOWNLOAD_DIR"
    mkdir -p $UNITY_DOWNLOAD_DIR
    curl -o $UNITY_PKG_LOCATION $UNITY_PKG_URL
    out "Finished Downloading Unity"

    out "Start Install Unity"
    sudo installer -dumplog -package $UNITY_PKG_LOCATION -target /

    if [ $? = 0 ] ; then
        out "Finished Install Unity"
    else
        die "Unable to install Unity"
    fi
fi
