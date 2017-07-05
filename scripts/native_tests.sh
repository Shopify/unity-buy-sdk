. $(dirname $0)/common.sh

# will copy all files needed for testing native extenstions
copy_native_tests() {
    mkdir -p $SCRIPTS_ROOT/native_tests/Assets/Plugins/iOS/Shopify/BuyTests/
    cp -r $SCRIPTS_ROOT/native_tests/Assets/Plugins/iOS/Shopify/ $PROJECT_ROOT/Assets/Plugins/iOS/Shopify/
}

# will delete all files needed for testing native extenstions
delete_native_tests() {
    rm -rf $PROJECT_ROOT/Assets/Plugins/iOS/Shopify/BuyTests/
    rm -f $PROJECT_ROOT/Assets/Plugins/iOS/Shopify/Unity-iPhone-Tests-Bridging-Header.h
}
