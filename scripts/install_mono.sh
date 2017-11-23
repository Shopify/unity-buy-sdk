MONO_PKG_URL="https://download.mono-project.com/archive/4.8.1/macos-10-universal/MonoFramework-MDK-4.8.1.0.macos10.xamarin.universal.pkg"
MONO_PKG_FILE="mono_4.8.1.pkg"

curl -o $MONO_PKG_FILE $MONO_PKG_URL

sudo installer -pkg $MONO_PKG_FILE -target /
