#!/bin/bash

. $(dirname $0)/common.sh

UNITY_LOG_PATH=$PROJECT_ROOT/test_integration.log
RESULT_PATH=$PROJECT_ROOT/results_integration

which $UNITY_PATH &> /dev/null || die "Unity does not exist at $UNITY_PATH" 

mkdir $RESULT_PATH

$UNITY_PATH \
    -batchmode \
    -projectPath $PROJECT_ROOT \
    -testscenes=TestScene \
    -logFile $UNITY_LOG_PATH \
    -resultsFileDirectory=$RESULT_PATH \
    -executeMethod UnityTest.Batch.RunIntegrationTests \
    -quit

# -targetPlatform=StandaloneWindows

if [ $? = 0 ] ; then
    echo "Integration tests passed ❤️"

    exit 0
elif [ $? = 2 ] ; then
    echo "Tests failed. Exited with $?"

    # echo "------------------\n\n"
    # if [ -e "EditorTestResults.xml" ]; then
    #     cat EditorTestResults.xml
    #    convertNUnitToJUnit
    # else
    #     cat $UNITY_LOG_PATH 
    # fi

    exit 1
else
    echo "Tests failed because of issues running tests. Exited with $?"
    
    exit 1
fi
