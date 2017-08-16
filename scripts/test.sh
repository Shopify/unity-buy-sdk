. "$(dirname "$0")"/common.sh

which mcs &> /dev/null || die "Mono is not installed"
which nunit-console &> /dev/null || die "nunit-console is not installed"

# we're defining SHOPIFY_TEST here so that source can check whether we're running assertions/tests
mcs \
    -debug \
    -define:SHOPIFY_TEST \
    -define:SHOPIFY_MONO_UNIT_TEST \
    -define:UNITY_IOS \
    -recurse:'Assets/Shopify/*.cs' \
    -recurse:'Assets/Editor/*.cs' \
    -reference:nunit.framework.dll \
    -target:library \
    -out:test.dll

MCS_RESULT=$?

if [[ $MCS_RESULT = 0 ]] ; then
    nunit-console test.dll
fi
