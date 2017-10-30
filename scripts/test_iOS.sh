#!/bin/bash

. "$(dirname "$0")"/common.sh

IOS_BUILD_PATH="$PROJECT_ROOT"/Shopify-iOS-Tests
IOS_PROJECT_PATH="$IOS_BUILD_PATH"/Unity-iPhone.xcodeproj
UNITY_IOS_LOG_PATH="$PROJECT_ROOT"/buildIOS.log

which "$UNITY_PATH" &> /dev/null || die "Unity does not exist at $UNITY_PATH"

"$UNITY_PATH" \
    -batchmode \
    -nographics \
    -silent-crashes \
    -logFile "$UNITY_IOS_LOG_PATH" \
    -projectPath "$PROJECT_ROOT" \
    -buildTarget ios \
    -executeMethod Shopify.Unity.Editor.BuildPipeline.ShopifyBuildPipeline.BuildIosForTests \
    -buildPlayerPath "$IOS_BUILD_PATH" \
    -quit

UNITY_BUILD_RESULT=$?

if [[ $UNITY_BUILD_RESULT = 0 ]] ; then
    printf "Project Built\n"
else
    cat "$UNITY_IOS_LOG_PATH"
fi

xcodebuild test \
    -project "$IOS_PROJECT_PATH" \
    -sdk iphonesimulator \
    -scheme Unity-iPhone \
    -destination 'platform=iOS Simulator,OS=11.0.1,name=iPhone 6'

IOS_TEST_RESULT=$?

if [[ $IOS_TEST_RESULT = 0 ]] ; then
    printf "Tests passed\n"
    exit 0
else
    printf "Tests failed. Exited with %s\n" "$IOS_TEST_RESULT"
    printf "------------------\n\n"
    exit 1
fi
