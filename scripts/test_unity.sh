#!/bin/bash

. $(dirname $0)/common.sh

UNITY_LOG_PATH=$PROJECT_ROOT/test.log
UNITY_TEST_RESULTS_PATH=$PROJECT_ROOT/EditorTestResults.xml

which $UNITY_PATH &> /dev/null || die "Unity does not exist at $UNITY_PATH" 

convertNUnitToJUnit() {
    if [ ! -z "${UNITY_CIRCLE_XML_OUT_PATH}" ]; then
        mkdir $UNITY_CIRCLE_XML_DIR
        xsltproc -o $UNITY_CIRCLE_XML_OUT_PATH $SCRIPTS_ROOT/nunit-to-junit.xsl $UNITY_TEST_RESULTS_PATH
    fi
}

$UNITY_PATH \
    -batchmode \
    -nographics \
    -silent-crashes \
    -logFile $UNITY_LOG_PATH \
    -projectPath $PROJECT_ROOT \
    -editorTestsResultFile EditorTestResults.xml \
    -runEditorTests \
    -buildTarget osx \
    -quit

if [ $? = 0 ] ; then
    echo "Tests passed"
    convertNUnitToJUnit

    exit 0
else
    echo "Tests failed. Exited with $?"
    echo "------------------\n\n"
    if [ -e $UNITY_TEST_RESULTS_PATH ]; then
        cat $UNITY_TEST_RESULTS_PATH
        convertNUnitToJUnit
    else
        cat $UNITY_LOG_PATH 
    fi
    exit 1
fi
