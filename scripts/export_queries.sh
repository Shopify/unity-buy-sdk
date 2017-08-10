. $(dirname $0)/common.sh

which mcs &> /dev/null || die "Mono is not installed"

# we're defining SHOPIFY_TEST here so that source can check whether we're running assertions/tests
mcs \
    -debug \
    -define:SHOPIFY_TEST \
    -define:SHOPIFY_MONO_UNIT_TEST \
    -define:UNITY_IOS \
    -recurse:'Assets/Shopify/*.cs' \
    -target:exe \
    -out:export.exe \
    ./scripts/export.cs

if [ $? = 0 ] ; then
    mono export.exe
fi
