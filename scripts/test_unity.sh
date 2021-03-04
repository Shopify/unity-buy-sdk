#!/bin/bash

. "$(dirname "$0")"/common.sh

UNITY_LOG_PATH="$PROJECT_ROOT"/test.log
UNITY_TEST_RESULTS_PATH="$PROJECT_ROOT"/EditorTestResults.xml

which "$UNITY_PATH" &> /dev/null || die "Unity does not exist at $UNITY_PATH"

convertNUnitToJUnit() {
    if [ ! -z "${UNITY_CIRCLE_XML_OUT_PATH}" ]; then
        mkdir "$UNITY_CIRCLE_XML_DIR"
        xsltproc -o "$UNITY_CIRCLE_XML_OUT_PATH" "$SCRIPTS_ROOT"/nunit-to-junit.xsl "$UNITY_TEST_RESULTS_PATH"
    fi
}

# UNITY_VERSION=$(ls "$UNITY_PACKAGE_MANAGER_PATH")
# printf "Testing with Unity Version: 2019.4.18f1"

UNITY_VERSION=$(ls "$UNITY_PACKAGE_MANAGER_PATH")
printf "Testing with Unity Version: %s\n" "$UNITY_VERSION"

"$UNITY_PATH" \
    -batchmode \
    -force-opengl \
    -silent-crashes \
    -logFile "$UNITY_LOG_PATH" \
    -projectPath "$PROJECT_ROOT" \
    -editorTestsResultFile ./EditorTestResults.xml \
    -runEditorTests

UNITY_BUILD_RESULT=$?

if [[ $UNITY_BUILD_RESULT = 0 ]] ; then
    printf "Tests passed\n"
    convertNUnitToJUnit

    exit 0
else
    printf "Tests failed. Exited with %s\n" "$UNITY_BUILD_RESULT"
    printf "%s\n\n" "------------------"
    if [[ -e $UNITY_TEST_RESULTS_PATH ]]; then
        cat "$UNITY_TEST_RESULTS_PATH"
        convertNUnitToJUnit
    else
        cat "$UNITY_LOG_PATH"
    fi
    exit 1
fi
