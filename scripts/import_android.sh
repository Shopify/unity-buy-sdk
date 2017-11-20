#!/bin/bash
FILE_PATH=$(echo "$1" | tr '[:upper:]' '[:lower:]')
if test "$#" -ne 1
then
    echo "Usage: import_android.sh file"
    exit 1
fi
if test -f $FILE_PATH
then
    if [ ${FILE_PATH: -4} == ".aar" ]
    then
        cp $FILE_PATH Assets/Shopify/Plugins/Android/libs/shopify_buy_plugin.aar
    else
        echo "Not an AAR file"
    fi
else
    echo "File not found: $FILE_PATH"
fi