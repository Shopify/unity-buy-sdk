. $(dirname $0)/common.sh

which doxygen &> /dev/null || die "doxygen is not installed"

doxygen Doxyfile

if [ $? = 0 ] ; then
    echo "🦄  Generated documentation 🦄"
    exit 0
else
    echo "😳 Generate documentation failed. Exited with $?"

    exit 1
fi
