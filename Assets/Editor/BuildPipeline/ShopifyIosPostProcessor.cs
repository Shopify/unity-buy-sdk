#if UNITY_EDITOR
namespace Shopify.BuildPipeline {
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.iOS.Xcode;
    using System.IO;
    using System;

    public class ShopifyIosPostProcessor {

        /// <summary>
        /// Perform post processing on the build to only run Shopify Tests
        /// </summary>
        public static void ProcessForTests(string buildPath) {
            var project = new ExtendedPBXProject(buildPath);
            SetBuildProperties(project);
            SetCorrectTestsTarget(project);
            project.RemoveFileFromBuild(project.TestTargetGuid, project.FindFileGuidByProjectPath("Unity-iPhone Tests/Unity_iPhone_Tests.m"));
            SetSwiftInterfaceHeader(project);
            project.Save();
        }

        /// Sets the correct build properties to run
        private static void SetBuildProperties(ExtendedPBXProject project) {
            project.SetBuildProperty(project.GetAllTargetGuids(), ExtendedPBXProject.SwiftVersionKey, "3.0");
            project.SetBuildProperty(project.UnityTargetGuid, ExtendedPBXProject.SwiftBridgingHeaderKey, "Libraries/Plugins/iOS/Shopify/Unity-iPhone-Bridging-Header.h");
            project.SetBuildProperty(project.TestTargetGuid, ExtendedPBXProject.SwiftBridgingHeaderKey, "Libraries/Plugins/iOS/Shopify/Unity-iPhone-Tests-Bridging-Header.h");
            project.SetBuildProperty(project.UnityTargetGuid, ExtendedPBXProject.RunpathSearchKey, "@executable_path/Frameworks");
            project.SetBuildProperty(project.TestTargetGuid, ExtendedPBXProject.RunpathSearchKey, "@loader_path/Frameworks");
            project.SetBuildProperty(project.TestTargetGuid, ExtendedPBXProject.ProjectModuleNameKey, "$(PRODUCT_NAME:c99extidentifier)Tests");
            project.SetBuildPropertyForConfig(project.GetDebugConfig(project.UnityTargetGuid), ExtendedPBXProject.EnableTestabilityKey, "YES");

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
                project.SetBuildProperty(project.GetAllTargetGuids(), ExtendedPBXProject.DeploymentTarget, "9.0");
            }
        }

        private static void SetSwiftInterfaceHeader(ExtendedPBXProject project) {
            var bundleIdentifier = PlayerSettings.iPhoneBundleIdentifier;

            char[] separator = {'.'};
            var keywords = bundleIdentifier.Split(separator);
            var productName = keywords[keywords.Length - 1];

            string swiftInterfaceHeaderPath = Path.Combine(project.BuildPath, "Libraries/Plugins/iOS/Shopify/SwiftInterfaceHeader.h");

            if (File.Exists(swiftInterfaceHeaderPath)) {
                string[] lines = File.ReadAllLines(swiftInterfaceHeaderPath, System.Text.Encoding.UTF8);

                if (lines.Length != 0) {
                    lines[0] = "#import \"" + productName + "-Swift.h\"";
                }

                File.WriteAllLines (swiftInterfaceHeaderPath, lines);
            }
        }

        /// Sets the correct target for Shopify Tests
        private static void SetCorrectTestsTarget(ExtendedPBXProject project) {
            string testPath = Path.Combine(project.BuildPath, "Libraries/Plugins/iOS/Shopify/BuyTests/");
            var testDirectory = new DirectoryInfo(testPath);

            try {
                project.SetFilesInDirectoryToTestTarget(testDirectory);
            } catch (Exception e) {
                Debug.Log(e.Message);
            }
        }
    }
}
#endif
