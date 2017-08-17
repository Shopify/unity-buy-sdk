#!/bin/bash

bundle --gemfile="$SCRIPTS_ROOT"/generator/Gemfile
"$SCRIPTS_ROOT"/generator/update_schema "$1"

UPDATE_SCHEMA_RESULT=$?

if [[ $UPDATE_SCHEMA_RESULT = 0 ]] ; then
    printf "Generate finished\n"
    exit 0
else
    printf "Generate failed. Exited with %s\n" "$?"

    exit 1
fi
