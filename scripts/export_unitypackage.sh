. $(dirname $0)/common.sh

UNITY_PATH="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
UNITY_LOG_PATH=$(pwd)/export.log

which $UNITY_PATH &> /dev/null || die "Unity does not exist at $UNITY_PATH" 

$UNITY_PATH \
    -batchmode \
    -silent-crashes \
    -logFile $UNITY_LOG_PATH \
    -projectPath $(pwd) \
    -exportPackage Assets/Shopify shopify-buy.unitypackage \
    -quit

if [ $? = 0 ] ; then
    echo "Export finished"
    exit 0
else
    echo "Export failed. Exited with $?"
    echo "------------------\n\n"
    cat $UNITY_LOG_PATH 

    exit 1
fi
