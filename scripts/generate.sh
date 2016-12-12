. $(dirname $0)/common.sh

bundle --gemfile=$SCRIPTS_ROOT/generator/Gemfile
$SCRIPTS_ROOT/generator/update_schema
