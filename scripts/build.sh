. $(dirname $0)/common.sh

which ruby &> /dev/null || die "Ruby is not installed"
which bundle &> /dev/null || die "Bundler is not installed"

start "generating GraphQL client"
$SCRIPTS_ROOT/generate.sh || die "Could not generate Ruby client"
end "generating GraphQL client"
