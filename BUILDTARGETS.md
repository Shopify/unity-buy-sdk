# Build Target Requirements

The Unity Buy SDK currently supports the following build targets:

* [iOS](#ios)

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



