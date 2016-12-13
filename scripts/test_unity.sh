. $(dirname $0)/common.sh

UNITY_PATH="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
UNITY_LOG_PATH=$(pwd)/test.log

which $UNITY_PATH &> /dev/null || die "Unity does not exist at $UNITY_PATH" 


$UNITY_PATH \
    -batchmode \
    -nographics \
    -silent-crashes \
    -logFile $UNITY_LOG_PATH \
    -projectPath $(pwd) \
    -runEditorTests \
    -runEditorTests \
    -quit

if [ $? = 0 ] ; then
    echo "Tests passed"
    exit 0
else
    echo "Tests failed. Exited with $?"
    echo "------------------\n\n"
    if [ -e "EditorTestResults.xml" ]; then
        cat EditorTestResults.xml
    else
        cat $UNITY_LOG_PATH 
    fi
    exit 1
fi