#!/bin/bash

. $(dirname $0)/common.sh

check() {
    if [ $? = 0 ] ; then
        echo "Export $@ finished properly."
    else
        echo "Export $@ failed. Exited with $?"
        echo "------------------\n\n"
        cat $UNITY_LOG_PATH 

        exit 1
    fi
}

get_package_name() {
    echo "shopify-buy-$1.unitypackage"
}

# check if we need to do a major, minor, patch update
VERSION=`cat $SCRIPTS_ROOT/version`

a=( ${VERSION//./ } )

if [ $# -eq 0 ] ; then
    echo "You must pass in either: major, minor, or patch"
    exit 1
elif [ $1 = "major" ] ; then
    ((a[0]++))
    a[1]=0
    a[2]=0
elif [ $1 = "minor" ] ; then
    ((a[1]++))
    a[2]=0
elif [ $1 = "patch" ] ; then
    ((a[2]++))
else
    echo "Invalid version identifier: \"$1\". You must pass in either: major, minor, or patch"
    exit 1
fi

PREVIOUS_VERSION=$VERSION
VERSION="${a[0]}.${a[1]}.${a[2]}"

echo $VERSION > $SCRIPTS_ROOT/version
echo "Bumped $1: $VERSION"

# The following will run generate to generate C# for client
$SCRIPTS_ROOT/generate.sh
check "generate"

# Run tests just in case
$SCRIPTS_ROOT/test.sh
check "test"

# Now we'll attempt to actually generate the unitypackage
UNITY_PATH="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
UNITY_LOG_PATH=$(pwd)/export.log

NEW_UNITYPACKAGE=$(get_package_name $VERSION)
PREVIOUS_UNITYPACKAGE=$(get_package_name $PREVIOUS_VERSION)

which $UNITY_PATH &> /dev/null || die "Unity does not exist at $UNITY_PATH" 

# delete the old unitypackage
rm *.unitypackage

# create the new unitypackage
$UNITY_PATH \
    -batchmode \
    -silent-crashes \
    -logFile $UNITY_LOG_PATH \
    -projectPath $(pwd) \
    -exportPackage Assets/Shopify $NEW_UNITYPACKAGE \
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
