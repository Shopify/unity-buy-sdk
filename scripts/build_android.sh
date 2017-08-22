#!/bin/bash

. "$(dirname "$0")"/common.sh

ANDROID_PLUGIN_ROOT="$PROJECT_ROOT/android"

start "generating Android Plugin"

# Run Unity assembly task
"$ANDROID_PLUGIN_ROOT"/gradlew shopify_buy_plugin:assembleUnity -p "$ANDROID_PLUGIN_ROOT" || die "Failed to generate Android AAR plugin"
