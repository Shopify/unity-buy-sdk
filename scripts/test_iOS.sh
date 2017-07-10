#!/bin/bash

. $(dirname $0)/common.sh

IOS_BUILD_PATH="$PROJECT_ROOT"/Shopify-iOS-Tests
IOS_PROJECT_PATH="$IOS_BUILD_PATH"/Unity-iPhone.xcodeproj
UNITY_IOS_LOG_PATH="$PROJECT_ROOT"/buildIOS.log

which "$UNITY_PATH" &> /dev/null || die "Unity does not exist at $UNITY_PATH" 

# Activate license
if [[ -n "$UNITY_USERNAME" && -n "$UNITY_PASSWORD" && -n "$UNITY_SERIAL"  ]] ; then
    echo "Unity building with user $UNITY_USERNAME"

    "$UNITY_PATH" \
        -batchmode \
        -nographics \
        -username "$UNITY_USERNAME" \
        -password "$UNITY_PASSWORD" \
        -serial "$UNITY_SERIAL" \
        -silent-crashes \
        -logFile "$UNITY_IOS_LOG_PATH" \
        -quit

    # Here https://docs.unity3d.com/Manual/CommandLineArguments.html
    # it states that activating the license might take a bit of time :(
    sleep 5
else
    echo "Unity building without user"   
fi

"$UNITY_PATH" \
    -batchmode \
    -nographics \
    -silent-crashes \
    -logFile "$UNITY_IOS_LOG_PATH" \
    -projectPath "$PROJECT_ROOT" \
    -executeMethod Shopify.BuildPipeline.ShopifyBuildPipeline.BuildIosForTests \
    -buildPlayerPath "$IOS_BUILD_PATH" \
    -quit

# Deactivate license
if [[ -n "$UNITY_USERNAME" && -n "$UNITY_PASSWORD" && -n "$UNITY_SERIAL"  ]] ; then
    echo "Deactivate license"

    "$UNITY_PATH" \
        -batchmode \
        -nographics \
        -returnlicense\
        -quit
fi

if [ $? = 0 ] ; then
    echo "Project Built"
else
    cat "$UNITY_IOS_LOG_PATH"
fi

xcodebuild test \
    -project "$IOS_PROJECT_PATH" \
    -sdk iphonesimulator \
    -scheme Unity-iPhone \
    -destination 'platform=iOS Simulator,OS=10.0,name=iPhone 6'

if [ $? = 0 ] ; then
    echo "Tests passed"
    exit 0
else
    echo "Tests failed. Exited with $?"
    echo "------------------\n\n"
    exit 1
fi
