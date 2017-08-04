#!/bin/bash

has_license_credentials() {
    if [[ -n $UNITY_USERNAME && -n $UNITY_PASSWORD && -n $UNITY_SERIAL  ]] ; then
        return 0
    else
        return 1
    fi
}
