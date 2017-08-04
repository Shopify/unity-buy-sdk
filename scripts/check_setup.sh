#!/bin/bash

. "$(dirname "$0")"/common.sh

COLOR_RED='\033[0;31m'
COLOR_GREEN='\033[0;32m'
BOLD='\033[1m'
RESET_STYLE='\033[0m'

HAS_RUBY=true
HAS_BUNDLER=true
HAS_MCS=true
HAS_NUNIT_CONSOLE=true
HAS_UNITY=true
HAS_NODE=true
HAS_NODEMON=true

which ruby &> /dev/null || HAS_RUBY=false
which bundle &> /dev/null || HAS_BUNDLER=false
which mcs &> /dev/null || HAS_MCS=false
which nunit-console &> /dev/null || HAS_MCS=false
which node &> /dev/null || HAS_NODE=false
which nodemon &> /dev/null || HAS_NODEMON=false
which "$UNITY_PATH" &> /dev/null || HAS_UNITY=false

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

print_bold_text() {
    # arg 1: String
    # arg 2: Color
    printf "%b$1%b" "$2$BOLD" "$RESET_STYLE"
}

print_point_form_prefix() {
    # arg 1: String
    printf "◦ %s : " "$(print_bold_text "$1")"
}

# Output required dependencies
if [[ $HAS_REQUIRED = false ]] ; then
    print_bold_text "\n\nThe following required required dependencies are missing:\n" "$COLOR_RED"
fi

if [[ $HAS_UNITY = false ]] ; then
    print_point_form_prefix "unity"
    printf "should be installed at the following path %s\n" "$UNITY_PATH"
fi

if [[ $HAS_RUBY = false ]] ; then
    print_point_form_prefix "ruby"
    printf "is used by 'scripts/build.sh'' to generate C# source for the SDK. Ruby 2.3.0 or greater is required\n"
fi

if [[ $HAS_BUNDLER = false ]] ; then
    print_point_form_prefix "bundle"
    printf "is used to install dependencies for the generator for more information checkout: "
    print_bold_text "http://bundler.io/ \n"
fi

if [[ $HAS_MCS = false ]] ; then
    print_point_form_prefix "mcs"
    printf "Mono C# compiler is required to compile C# for local testing. This is not required if all testing is done via Unity\n"
fi

if [[ $HAS_NUNIT_CONSOLE = false ]] ; then
    print_point_form_prefix "nunit-console"
    printf "is required to output results from tests\n"
fi


# Output optional dependencies
if [[ $HAS_OPTIONAL = false ]] ; then
    print_bold_text "\n\nThe following are optional dependencies which are missing:\n" "$COLOR_RED"
fi

if [[ $HAS_NODE = false ]] ; then
    print_point_form_prefix "node"
    printf "is used when running 'scripts/test_watch.sh'. scripts/test_watch.sh will watch all development files, rebuild when files changes, \n"
    printf "and finally run tests. Basically this script allows for quick local development\n"
fi

if [[ $HAS_NODEMON = false ]] ; then
    print_point_form_prefix "nodemon"
    printf "watches files and runs a script when files are changed. In specific "
    print_bold_text "nodemon"
    printf " is used by 'scripts/test_watch.sh'.\n For more information checkout: "
    print_bold_text "http://npmjs.com/nodemon \n"
fi

if [[ $HAS_OPTIONAL = false ]] ; then
    printf "\n\n"
fi

if [[ $HAS_OPTIONAL = true && $HAS_REQUIRED ]] ; then
    print_bold_text "You are good to go\n" "$COLOR_GREEN"
fi
