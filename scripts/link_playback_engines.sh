. $(dirname $0)/common.sh

UNITY_PLAYBACK_ENGINES_PATH=$UNITY_APP_PATH/../PlaybackEngines
PROJECT_PLAYBACK_ENGINES_PATH=$PROJECT_ROOT/Assets

if [ ! -d "$UNITY_PLAYBACK_ENGINES_PATH" ]; then 
    die "There are no PlaybackEngines at $UNITY_PLAYBACK_ENGINES_PATH";
fi 

sudo ln -s $UNITY_PLAYBACK_ENGINES_PATH $PROJECT_PLAYBACK_ENGINES_PATH

if [ $? = 0 ] ; then
    echo "Linked PlaybackEngines from $UNITY_PLAYBACK_ENGINES_PATH to $PROJECT_PLAYBACK_ENGINES_PATH"
    exit 0
else
    echo "Unable to link PlaybackEngines. Exited with $?"
    echo "------------------\n\n"
    exit 1
fi
