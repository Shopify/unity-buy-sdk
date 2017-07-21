. $(dirname $0)/common.sh

which watchman-make &> /dev/null || die "watchman/watchman-make is not installed"

ANDROID_ROOT="$PROJECT_ROOT/Assets/Editor/Android"
AAR_TARGET_NAME="BuyAndroidPlugin"

watchman-make --root "$ANDROID_ROOT" --make buck -p "**/*.java" -t "test -all"
