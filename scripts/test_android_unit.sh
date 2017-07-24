. $(dirname $0)/common.sh

ANDROID_PLUGIN_ROOT="$PROJECT_ROOT/unity_buy_android"

# Run Unity assembly task
"$ANDROID_PLUGIN_ROOT"/gradlew unity_buy_plugin:testDebug -p "$ANDROID_PLUGIN_ROOT" || die "Failed to test Android plugin"
