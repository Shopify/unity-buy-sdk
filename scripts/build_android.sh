. $(dirname $0)/common.sh

which buck &> /dev/null || die "Buck is not installed"

ANDROID_ROOT="$PROJECT_ROOT/Assets/Editor/Android"
AAR_TARGET_NAME="BuyAndroidPlugin"
AAR_OUTPUT_PATH="$ANDROID_ROOT/buck-out/gen/$AAR_TARGET_NAME.aar"
PLUGINS_ROOT="$PROJECT_ROOT/Assets/Plugins"

start "building Android aar for native plugin"

cd "$ANDROID_ROOT" || exit
buck build :$AAR_TARGET_NAME || 
cd "$PROJECT_ROOT" || exit

cp -r "$AAR_OUTPUT_PATH" "$PLUGINS_ROOT/Android/$AAR_TARGET_NAME.aar"



