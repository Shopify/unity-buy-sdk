# Build Target Requirements

The Unity Buy SDK currently supports the following build targets:

* [Android](#Android)
* [iOS](#ios)

# Android

### Requirements

The Unity Buy SDK requires Android games to have a minimum API level of Android 4.4 'Kit Kat' (API Level 19)

### Building

The library comes in a form of a `.AAR` file. In doing so we have provided a [mainTemplate.gradle](Assets/Plugins/Android/mainTemplate.gradle) that will be used by Unity when creating a Gradle project.

The Gradle file includes the dependencies used by the libarary. If you would like to use your own Gradle file, add the following dependencies:

```
dependencies {
    ...

    compile(name: 'shopify_buy_plugin', ext:'aar')

    compile 'com.android.support:appcompat-v7:26.0.0'
    compile 'com.android.support:customtabs:26.0.0'
    compile 'com.shopify.mobilebuysdk:buy3-pay-support:1.0.0'
    compile 'com.squareup.moshi:moshi:1.5.0'
}
```


# iOS

### Requirements

The Unity Buy SDK requires iOS games to have a minimum target SDK of iOS 9 and support Swift 3.

### Building

When running an iOS Player build, the SDK runs a custom post processor that will make changes to the generated Xcode project to support the SDK's requirements. These changes include:

* Updating the Xcode project to support a minimum target of iOS 9.
* Changing the default Swift supported version from 2 to 3.
* Linking in a bridging header for the generated Unity target.
* Modifications to the target's Runpath.

For more details on the iOS post processor, see `Shopify/Editor/BuildPipeline/iOSPostProcessor.cs` after generating the project.



