# Build Details

The Shopify SDK for Unity uses Unity features to modify certain files during the build phase when building for a specific platform.

These modifications ensure that the SDK is properly configured for ease of use. Below is a more detailed explanation on what these modifications are.

# Android

The library comes in a form of a `.AAR` file. In doing so we have provided a [mainTemplate.gradle](Assets/Shopify/Plugins/Android/mainTemplate.gradle) that will be used by Unity when creating a Gradle project.

The Gradle file includes the dependencies used by the library. If you would like to use your own Gradle file, add the following dependencies:

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

When running an iOS Player build, the SDK runs a custom post processor that will make changes to the generated Xcode project to support the SDK's requirements. These changes include:

* Updating the Xcode project to support a minimum target of iOS 9.
* Changing the default Swift supported version from 2 to 3.
* Linking in a bridging header for the generated Unity target.
* Modifications to the target's Runpath.

For more details on the iOS post processor, see `Shopify/Editor/BuildPipeline/iOSPostProcessor.cs` after generating the project.


