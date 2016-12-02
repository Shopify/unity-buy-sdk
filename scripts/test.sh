. $(dirname $0)/common.sh

which mcs &> /dev/null || die "Mono is not installed"
which nunit-console &> /dev/null || die "nunit-console is not installed"

mcs -recurse:'*.cs' -reference:nunit.framework.dll -target:library -out:test.dll
nunit-console test.dll
