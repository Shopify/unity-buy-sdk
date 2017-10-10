#!/bin/bash

. "$(dirname "$0")"/common.sh

# will delete all files needed for testing native extenstions
delete_native_tests() {
    rm -rf "$PROJECT_ROOT/Assets/Shopify/Plugins/iOS/Shopify/BuyTests/"
    rm -f "$PROJECT_ROOT/Assets/Shopify/Plugins/iOS/Shopify/Unity-iPhone-Tests-Bridging-Header.h"
}

# will restore all delete files needed for testing native extenstions
restore_native_tests() {
    git checkout "$PROJECT_ROOT"/Assets/Shopify/Plugins/iOS/Shopify/BuyTests*
    git checkout "$PROJECT_ROOT"/Assets/Shopify/Plugins/iOS/Shopify/Unity-iPhone-Tests-Bridging-Header.h*
}
