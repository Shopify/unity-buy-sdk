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
            AddUiKitImportHeader(project);
            project.RemoveFileFromBuild(project.TestTargetGuid, project.FindFileGuidByProjectPath("Unity-iPhone Tests/Unity_iPhone_Tests.m"));
            project.Save();
        }

        /// Sets the correct build properties to run
        private static void SetBuildProperties(ExtendedPBXProject project) {
            project.SetBuildProperty(project.GetAllTargetGuids(), ExtendedPBXProject.SwiftVersionKey, "3.0");
            project.SetBuildProperty(project.GetAllTargetGuids(), ExtendedPBXProject.SwiftBridgingHeaderKey, "Libraries/Plugins/iOS/Shopify/Unity-iPhone-Bridging-Header.h");
            project.SetBuildProperty(project.UnityTargetGuid, ExtendedPBXProject.RunpathSearchKey, "@executable_path/Frameworks");
            project.SetBuildProperty(project.TestTargetGuid, ExtendedPBXProject.RunpathSearchKey, "@loader_path/Frameworks");
            project.SetBuildProperty(project.TestTargetGuid, ExtendedPBXProject.ProjectModuleNameKey, "$(PRODUCT_NAME:c99extidentifier)Tests");
            project.SetBuildPropertyForConfig(project.GetDebugConfig(project.UnityTargetGuid), ExtendedPBXProject.EnableTestabilityKey, "YES");

            bool isBelowMinimumTarget = false;

            #if UNITY_5_5_OR_NEWER
            if (UnityEditor.PlayerSettings.iOS.targetOSVersionString == null || float.Parse(UnityEditor.PlayerSettings.iOS.targetOSVersionString) < 9.0) {
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

        /// Adds #import <UIKit/UIKit.h> to UnityAppController.h
        private static void AddUiKitImportHeader(ExtendedPBXProject project) {
            string appControllerHeaderPath = Path.Combine(project.BuildPath, "Classes/UnityAppController.h");

            if (File.Exists(appControllerHeaderPath)) {
                string[] lines = File.ReadAllLines (appControllerHeaderPath, System.Text.Encoding.UTF8);

                if (lines.Length != 0) {
                    lines[0] = String.Concat(lines[0], "\n#import <UIKit/UIKit.h>");
                }

                File.WriteAllLines (appControllerHeaderPath, lines);
            }
        }
    }
}
#endif
