#if UNITY_EDITOR && UNITY_IOS
namespace Shopify.Unity.Editor.BuildPipeline {
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.iOS.Xcode;
    using System.IO;
    using System;

    public class iOSTestPostProcessor {

        /// <summary>
        /// Perform post processing on the build to only run Shopify Tests
        /// </summary>
        public static void ProcessForTests(string buildPath) {
            var project = new ExtendedPBXProject(buildPath);
            SetBuildProperties(project);
            SetCorrectTestsTarget(project);
            project.RemoveFileFromBuild(project.TestTargetGuid, project.FindFileGuidByProjectPath("Unity-iPhone Tests/Unity_iPhone_Tests.m"));
            iOSPostProcessor.SetSwiftInterfaceHeader(project);
            project.Save();
        }

        /// Sets the correct build properties to run
        private static void SetBuildProperties(ExtendedPBXProject project) {
            iOSPostProcessor.SetBuildProperties(project);
            project.SetBuildProperty(project.GetAllTargetGuids(), ExtendedPBXProject.SwiftVersionKey, "4.0");
            project.SetBuildProperty(project.TestTargetGuid, ExtendedPBXProject.SwiftBridgingHeaderKey, "Libraries/Shopify/Plugins/iOS/Shopify/Unity-iPhone-Tests-Bridging-Header.h");
            project.SetBuildProperty(project.TestTargetGuid, ExtendedPBXProject.RunpathSearchKey, "@loader_path/Frameworks");
            project.SetBuildProperty(project.TestTargetGuid, ExtendedPBXProject.ProjectModuleNameKey, "$(PRODUCT_NAME:c99extidentifier)Tests");
            project.SetBuildPropertyForConfig(project.GetDebugConfig(project.UnityTargetGuid), ExtendedPBXProject.EnableTestabilityKey, "YES");
            project.SetBuildPropertyForConfig(project.GetDebugConfig(project.UnityTargetGuid), ExtendedPBXProject.SwiftOptimizationLevelKey, "-Onone");
            project.SetBuildPropertyForConfig(project.GetDebugConfig(project.UnityTargetGuid), ExtendedPBXProject.ClangAllowNonModularIncludesInFrameworkModules, "YES");
        }

        /// Sets the correct target for Shopify Tests
        private static void SetCorrectTestsTarget(ExtendedPBXProject project) {
            string testPath = Path.Combine(project.BuildPath, "Libraries/Shopify/Plugins/iOS/Shopify/BuyTests/");
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
