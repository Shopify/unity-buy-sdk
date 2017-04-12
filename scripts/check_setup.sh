. $(dirname $0)/common.sh

COLOR_RED='\033[0;31m'
BOLD='\033[1m'
RESET_STYLE='\033[0m'

HAS_RUBY=false
HAS_BUNDLER=false
HAS_MCS=false
HAS_NUNIT_CONSOLE=false
HAS_UNITY=false
HAS_NODE=false
HAS_NODEMON=false

which ruby &> /dev/null || HAS_RUBY=false
which bundle &> /dev/null || HAS_BUNDLER=false
which mcs &> /dev/null || HAS_MCS=false
which nunit-console &> /dev/null || HAS_MCS=false
which node &> /dev/null || HAS_NODE=false
which nodemon &> /dev/null || HAS_NODEMON=false
which $UNITY_PATH &> /dev/null || HAS_UNITY=false

if [[ $HAS_UNITY = true && $HAS_RUBY = true && $HAS_BUNDLER = true && $HAS_MCS = true && $HAS_NUNIT_CONSOLE = true ]] ; then
    HAS_REQUIRED=true
else
    HAS_REQUIRED=false
fi

if [[ $HAS_NODE = true && $HAS_NODEMON = true ]] ; then
    HAS_OPTIONAL=true
else
    HAS_OPTIONAL=false
fi

# Output required dependencies
if [ $HAS_REQUIRED = false ] ; then
    echo "\n\n${COLOR_RED}${BOLD}The following required required dependencies are missing:${RESET_STYLE}\n"
fi

if [ $HAS_UNITY = false ] ; then
    echo "◦ ${BOLD}unity${RESET_STYLE}: should be installed at the following path $UNITY_PATH\n"
fi

if [ $HAS_RUBY = false ] ; then
    echo "◦ ${BOLD}ruby${RESET_STYLE}: is used by 'scripts/build.sh'' to generate C# source for the SDK. Ruby 2.3.0 or greater is required\n"
fi

if [ $HAS_BUNDLER = false ] ; then
    echo "◦ ${BOLD}bundle${RESET_STYLE}: is used to install dependencies for the generator for more information checkout: ${BOLD}http://bundler.io/${RESET_STYLE}\n"
fi

if [ $HAS_MCS = false ] ; then
    echo "◦ ${BOLD}mcs${RESET_STYLE}: Mono C# compiler is required to compile C# for local testing. This is not required if all testing is done via Unity\n"
fi

if [ $HAS_NUNIT_CONSOLE = false ] ; then
    echo "◦ nunit-console: is required to output results from tests\n"
fi


# Output optional dependencies
if [ $HAS_OPTIONAL = false ] ; then
    echo "\n\n${COLOR_RED}${BOLD}The following are optional dependencies which are missing:${RESET_STYLE}\n"
fi

if [ $HAS_NODE = false ] ; then
    echo "◦ ${BOLD}node${RESET_STYLE}: is used when running 'scripts/test_watch.sh'. scripts/test_watch.sh will watch all development files, rebuild when files changes,\n" \
    "and finally run tests. Basically this script allows for quick local development\n"
fi

if [ $HAS_NODEMON = false ] ; then
    echo "◦ ${BOLD}nodemon${RESET_STYLE}: watches files and runs a script when files are changed. In specific ${BOLD}nodemon${RESET_STYLE} is used by 'scripts/test_watch.sh'.\n" \
    "For more information checkout: ${BOLD}http://npmjs.com/nodemon${RESET_STYLE}"
fi

if [ $HAS_OPTIONAL = false ] ; then
    echo "\n\n"
fi
