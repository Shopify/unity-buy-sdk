#!/bin/bash

. $(dirname $0)/common.sh

IOS_BUILD_PATH="$PROJECT_ROOT"/Shopify-iOS-Tests
IOS_PROJECT_PATH="$IOS_BUILD_PATH"/Unity-iPhone.xcodeproj
IOS_TEST_ASSETS="$PROJECT_ROOT"/iOSTestAssets
UNITY_ASSETS="$PROJECT_ROOT"/Assets
UNITY_IOS_LOG_PATH="$PROJECT_ROOT"/buildIOS.log

which "$UNITY_PATH" &> /dev/null || die "Unity does not exist at $UNITY_PATH" 

cp -r "$IOS_TEST_ASSETS" "$UNITY_ASSETS"/iOSTestAssets

"$UNITY_PATH" \
    -batchmode \
    -nographics \
    -silent-crashes \
    -logFile "$UNITY_IOS_LOG_PATH" \
    -projectPath "$PROJECT_ROOT" \
    -executeMethod Shopify.BuildPipeline.ShopifyBuildPipeline.BuildIosForTests \
    -buildPlayerPath "$IOS_BUILD_PATH" \
    -quit

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
