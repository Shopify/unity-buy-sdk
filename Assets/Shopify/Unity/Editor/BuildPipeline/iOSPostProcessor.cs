#if UNITY_EDITOR && UNITY_IOS
namespace Shopify.Unity.Editor.BuildPipeline {
    using System.IO;
    using System;
    using UnityEditor.Callbacks;
    using UnityEditor.iOS.Xcode;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Custom iOS build post processor that modifies the exported Xcode project to support the SDK.
    /// </summary>
    public class iOSPostProcessor {
        [PostProcessBuildAttribute(0)]
        public static void ProcessForExport(BuildTarget target, string buildPath) {
            if (target != BuildTarget.iOS)
                return;

            var project = new ExtendedPBXProject(buildPath);
            SetCorrectTestsTarget(project);
            SetBuildProperties(project);
            SetSwiftInterfaceHeader(project);
            project.Save();
        }

        // Sets the required project settings for the Xcode project to work with the SDK.
        public static void SetBuildProperties(ExtendedPBXProject project) {
            project.SetBuildProperty(project.GetAllTargetGuids(), ExtendedPBXProject.SwiftVersionKey, "4.0");
            project.SetBuildProperty(project.UnityTargetGuid, ExtendedPBXProject.RunpathSearchKey, "@executable_path/Frameworks");

            project.AddBuildProperty(project.GetUnityFrameworkTargetGuid(), ExtendedPBXProject.LibrarySearchPathsKey, "$(inherited)");
            project.AddBuildProperty(project.GetUnityFrameworkTargetGuid(), ExtendedPBXProject.LibrarySearchPathsKey, "$(TOOLCHAIN_DIR)/usr/lib/swift-5.0/$(PLATFORM_NAME)");
            project.AddBuildProperty(project.GetUnityFrameworkTargetGuid(), ExtendedPBXProject.LibrarySearchPathsKey, "$(TOOLCHAIN_DIR)/usr/lib/swift/$(PLATFORM_NAME)");

            bool isBelowMinimumTarget = false;

#if UNITY_5_5_OR_NEWER
            if (UnityEditor.PlayerSettings.iOS.targetOSVersionString == null ||
                UnityEditor.PlayerSettings.iOS.targetOSVersionString.Length == 0 ||
                float.Parse(UnityEditor.PlayerSettings.iOS.targetOSVersionString) < 9.0) {
                isBelowMinimumTarget = true;
            }
#elif UNITY_5_4_2_OR_NEWER
            isBelowMinimumTarget = UnityEditor.PlayerSettings.iOS.targetOSVersion < iOSTargetOSVersion.iOS_9_0;
#else
            isBelowMinimumTarget = true;
#endif

            if (isBelowMinimumTarget) {
                project.SetBuildProperty(project.GetAllTargetGuids(), ExtendedPBXProject.DeploymentTarget, "10.0");
            }
        }

        public static void SetSwiftInterfaceHeader(ExtendedPBXProject project) {
            var bundleIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);

            char[] separator = { '.' };
            var keywords = bundleIdentifier.Split(separator);
            var productName = keywords[keywords.Length - 1];

            string swiftInterfaceHeaderPath = Path.Combine(project.BuildPath, "Libraries/Shopify/Plugins/iOS/Shopify/SwiftInterfaceHeader.h");

            if (File.Exists(swiftInterfaceHeaderPath)) {
                string[] lines = File.ReadAllLines(swiftInterfaceHeaderPath, System.Text.Encoding.UTF8);

                if (lines.Length != 0) {
                    lines[0] = "#import \"UnityFramework/UnityFramework-Swift.h\"";
                }

                File.WriteAllLines(swiftInterfaceHeaderPath, lines);
            }
        }

        /// Sets the correct target for Shopify Tests
        private static void SetCorrectTestsTarget(ExtendedPBXProject project) {
            string testPath = Path.Combine(project.BuildPath, "Libraries/Shopify/Plugins/iOS/Shopify/BuyTests/");
            string testerPath = Path.Combine(project.BuildPath, "Libraries/Shopify/Plugins/iOS/Shopify/BuyTests/UnityTestHelpers/Tester.mm");
            var testDirectory = new DirectoryInfo(testPath);

            try {
                project.SetFilesInDirectoryToTestTarget(testDirectory);

                // Re-add Tester.mm file to main target to allow Tester.cs to find it's DllImport.
                project.AddFileToMainTarget(testerPath);
            } catch (Exception e) {
                Debug.Log(e.Message);
            }
        }
    }
}
#endif
