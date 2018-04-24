# Build Details

The SDK uses Unity features to modify certain files during the build phase when building for a specific platform.

These modifications ensure that the SDK is properly configured for ease of use. Below is a more detailed explanation on what these modifications are.

# Android

The library comes in the form of a `.AAR` file and doesn't require any special handling to build on Android.

# iOS

When running an iOS Player build, the SDK runs a custom post processor that will make changes to the generated Xcode project to support the SDK's requirements. These changes include:

* Updating the Xcode project to support a minimum target of iOS 9.
* Changing the default Swift supported version from 2 to 3.
* Linking in a bridging header for the generated Unity target.
* Modifications to the target's Runpath.

For more details on the iOS post processor, see `Shopify/Editor/BuildPipeline/iOSPostProcessor.cs` after generating the project.


