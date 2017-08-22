#!/bin/bash

. "$(dirname "$0")"/common.sh

which ruby &> /dev/null || die "Ruby is not installed"
which bundle &> /dev/null || die "Bundler is not installed"

# default to github publish destination if none is provided
if [[ $# -eq 0 ]] ; then
    PUBLISH_DESTINATION=github
else
    PUBLISH_DESTINATION="$1"
fi

start "generating GraphQL client"
"$SCRIPTS_ROOT"/generate.sh "$PUBLISH_DESTINATION" || die "Could not generate Ruby client"

end "generating GraphQL client"
