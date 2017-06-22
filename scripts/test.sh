. $(dirname $0)/common.sh

which mcs &> /dev/null || die "Mono is not installed"
which nunit-console &> /dev/null || die "nunit-console is not installed"

UNITY_MONO_DIR="/Applications/Unity/MonoDevelop.app/Contents/Frameworks/Mono.framework/Versions/Current"

# we're defining SHOPIFY_TEST here so that source can check whether we're running assertions/tests
"$UNITY_MONO_DIR/bin/mcs" -debug -define:SHOPIFY_TEST -define:SHOPIFY_MONO_UNIT_TEST -recurse:'Assets/Shopify/*.cs' -recurse:'Assets/Editor/*.cs' -reference:nunit.framework.dll -target:library -out:test.dll

if [ $? = 0 ] ; then
    nunit-console test.dll
fi
