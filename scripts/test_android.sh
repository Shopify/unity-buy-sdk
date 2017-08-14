#!/bin/bash

. "$(dirname "$0")"/common.sh

ANDROID_PLUGIN_ROOT="$PROJECT_ROOT"/android

"$ANDROID_PLUGIN_ROOT"/gradlew shopify_buy_plugin:check -p "$ANDROID_PLUGIN_ROOT" || die "Failed to check Android plugin"
