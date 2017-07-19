#!/bin/bash
# Sourced on CI

set -ex pipefail

echo "--- Exporting environment variables..."
export PROJECT_ROOT=$PWD
export UNITY_DOWNLOAD_DIR=$PROJECT_ROOT/unity
export UNITY_PKG_LOCATION=$UNITY_DOWNLOAD_DIR/Unity.pkg
export UNITY_PKG_URL=http://netstorage.unity3d.com/unity/b7e030c65c9b/MacEditorInstaller/Unity.pkg
export IOS_PKG_LOCATION=$UNITY_DOWNLOAD_DIR/Unity-iOS.pkg
export IOS_PKG_URL=http://netstorage.unity3d.com/unity/b7e030c65c9b/MacEditorTargetInstaller/UnitySetup-iOS-Support-for-Editor-5.4.2f2.pkg
export UNITY_CIRCLE_XML_DIR=$CIRCLE_TEST_REPORTS/Unity
export UNITY_CIRCLE_XML_OUT_PATH=$UNITY_CIRCLE_XML_DIR/junit.xml

echo "--- Booting simulator..."
xcrun instruments -w "iPhone 6 (10.3.1) [" || true

echo "--- Installing Unity"
./scripts/install_unity.sh


