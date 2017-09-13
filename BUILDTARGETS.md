# Build Target Requirements

The Unity Buy SDK currently supports the following build targets:

* [Android](#Android)
* [iOS](#ios)

You can find a more detailed explanation regarding platform specific dependencies and the build phase in [Build Details](BUILDDETAILS.md)

# Android

The Unity Buy SDK requires at least Unity 5.6.1 and Android games to have a minimum API level of Android 4.4 'Kit Kat' (API Level 19).

## Building with Gradle (Known Issues)

If you're looking to use the new Gradle build system introduced in Unity 5.6, you may run into the following:

***Building for release error***
* Error: `Avoid hardcoding the debug mode; leaving it out allows debug and release builds to automatically assign one [HardcodedDebugMode]`

When using the Gradle build system in Unity 5.6.1, Unity adds the `android:debuggable` attribute to the AndroidManifest.xml file which causes issues with the linter. As a workaround,

1. Select the `Export Project` setting in the Build Settings window and export the Gradle project.
2. Import the Android project into Android Studio by selecting `New` -> `Import Project..` and selecting the exported Unity folder.
3. Update the classpath in `gradle-wrapper.properties` inside the `<YourProject>/gradle/wrapper` folder to use Gradle 2.3.3 instead of 2.1.
4. Generate the release signed APK using `Build` -> `Release signed APK...`

Alternatively you can use the default `Internal` build system.

# iOS

The Unity Buy SDK requires iOS games to have a minimum target SDK of iOS 9 and support Swift 3.
