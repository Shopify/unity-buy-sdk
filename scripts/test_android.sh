. $(dirname $0)/common.sh

ANDROID_PLUGIN_ROOT="$PROJECT_ROOT/unity_buy_android"

"$ANDROID_PLUGIN_ROOT"/gradlew unity_buy_plugin:check -p "$ANDROID_PLUGIN_ROOT" || die "Failed to check Android plugin"
"$ANDROID_PLUGIN_ROOT"/gradlew unity_buy_plugin:test -p "$ANDROID_PLUGIN_ROOT" || die "Failed to test Android plugin"
