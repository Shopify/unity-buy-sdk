. $(dirname $0)/common.sh

which doxygen &> /dev/null || die "doxygen is not installed"

# Assets/Shopify/examples.md will be the main page for Doxygen
cp Assets/Shopify/examples.txt Assets/Shopify/examples.md
doxygen Doxyfile
rm Assets/Shopify/examples.md

if [ $? = 0 ] ; then
    echo "🦄  Generated documentation 🦄"
    exit 0
else
    echo "😳 Generate documentation failed. Exited with $?"

    exit 1
fi
