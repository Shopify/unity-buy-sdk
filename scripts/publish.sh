#!/bin/bash

. $(dirname $0)/common.sh
. $(dirname $0)/native_tests.sh

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

# delete all files required for native testing
delete_native_tests

# check if we need to do a major, minor, patch update
VERSION=`cat $(dirname $0)/version`

VERSION_ARRAY=( ${VERSION//./ } )

if [ $# -eq 0 ] ; then
    echo "If you'd like to bump versions pass: major, minor, or patch. Using $VERSION for now."
elif [ $1 = "major" ] ; then
    ((VERSION_ARRAY[0]++))
    VERSION_ARRAY[1]=0
    VERSION_ARRAY[2]=0
elif [ $1 = "minor" ] ; then
    ((VERSION_ARRAY[1]++))
    VERSION_ARRAY[2]=0
elif [ $1 = "patch" ] ; then
    ((VERSION_ARRAY[2]++))
else
    echo "Invalid version identifier: \"$1\". You must pass in either: major, minor, or patch"
    exit 1
fi

VERSION="${VERSION_ARRAY[0]}.${VERSION_ARRAY[1]}.${VERSION_ARRAY[2]}"

echo $VERSION > $(dirname $0)/version
echo "Version used $1: $VERSION"

# Run generate.sh to create source code including the potentially new version number
$(dirname $0)/generate.sh
check "generate"

# Run tests just in case
$(dirname $0)/test.sh
check "test"

# Now we'll attempt to actually generate the unitypackage
UNITY_LOG_PATH=$PROJECT_ROOT/export.log

which $UNITY_PATH &> /dev/null || die "Unity does not exist at $UNITY_PATH"

$(dirname $0)/build_documentation.sh
check "docs"

# copy EXAMPLES.md to Assets/Shopify/examples.txt
cp $PROJECT_ROOT/EXAMPLES.md $PROJECT_ROOT/Assets/Shopify/examples.txt

# create the new unitypackage
$UNITY_PATH \
    -batchmode \
    -nographics \
    -silent-crashes \
    -logFile $UNITY_LOG_PATH \
    -projectPath $PROJECT_ROOT \
    -exportPackage Assets/Shopify Assets/Plugins shopify-buy.unitypackage \
    -quit

# restore files used for native extensions
restore_native_tests

if [ $? = 0 ] ; then
    # clean up examples.txt
    rm $PROJECT_ROOT/Assets/Shopify/examples.txt

    echo "Export finished"
else
    echo "Export failed. Exited with $?"
    echo "------------------\n\n"
    cat $UNITY_LOG_PATH

    exit 1
fi
