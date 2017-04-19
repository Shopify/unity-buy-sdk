. $(dirname $0)/common.sh

which doxygen &> /dev/null || die "doxygen is not installed"

cat $PROJECT_ROOT/scripts/docs/temp_landing.md > $PROJECT_ROOT/scripts/docs/landing.md
cat $PROJECT_ROOT/EXAMPLES.md >> $PROJECT_ROOT/scripts/docs/landing.md

doxygen Doxyfile

if [ $? = 0 ] ; then
    echo "🦄  Generated documentation 🦄"
    exit 0
else
    echo "😳 Generate documentation failed. Exited with $?"

    exit 1
fi
