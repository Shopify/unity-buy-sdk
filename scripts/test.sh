. $(dirname $0)/common.sh

which mcs &> /dev/null || die "Mono is not installed"
which nunit-console &> /dev/null || die "nunit-console is not installed"

mcs -debug -recurse:'Assets/Shopify/*.cs' -recurse:'Assets/Editor/*.cs' -reference:nunit.framework.dll -target:library -out:test.dll

if [ $? = 0 ] ; then
    nunit-console test.dll
fi
