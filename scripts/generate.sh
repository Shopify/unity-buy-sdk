. $(dirname $0)/common.sh

bundle --gemfile=$SCRIPTS_ROOT/generator/Gemfile
$SCRIPTS_ROOT/generator/update_schema

if [ $? = 0 ] ; then
    echo "Generate finished"
    exit 0
else
    echo "Generate failed. Exited with $?"

    exit 1
fi
