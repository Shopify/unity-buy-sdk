#if UNITY_EDITOR && UNITY_IOS
namespace Shopify.Unity.Editor.BuildPipeline {
    using System.IO;
    using System;
    using UnityEditor.iOS.Xcode;
    using UnityEngine;

    /// <summary>
    /// A subclass on PBXProject providing convenience functions for configuring the project
    /// </summary>
    public class ExtendedPBXProject : PBXProject {
        public static string SwiftVersionKey = "SWIFT_VERSION";
        public static string DeploymentTarget = "IPHONEOS_DEPLOYMENT_TARGET";
        public static string RunpathSearchKey = "LD_RUNPATH_SEARCH_PATHS";
        public static string ProjectModuleNameKey = "PRODUCT_MODULE_NAME";
        public static string EnableTestabilityKey = "ENABLE_TESTABILITY";
        public static string SwiftBridgingHeaderKey = "SWIFT_OBJC_BRIDGING_HEADER";
        public static string SwiftOptimizationLevelKey = "SWIFT_OPTIMIZATION_LEVEL";
        public static string LibrarySearchPathsKey = "LIBRARY_SEARCH_PATHS";
        public static string EnableBitcodeKey = "ENABLE_BITCODE";
        public static string ClangAllowNonModularIncludesInFrameworkModules = "CLANG_ALLOW_NON_MODULAR_INCLUDES_IN_FRAMEWORK_MODULES";

        public readonly string BuildPath;
        public readonly string TestTargetGuid;
        public readonly string UnityTargetGuid;

        /// <summary>
        /// Creates a ExtendedPBXProject from .xcodeproj
        /// </summary>
        /// <param name="buildPath">Unity Project build path</param>
        public ExtendedPBXProject(string buildPath) : base() {
            this.BuildPath = buildPath;
            ReadFromFile(PBXProject.GetPBXProjectPath(buildPath));
            TestTargetGuid = TargetGuidByName(PBXProject.GetUnityTestTargetName());
            UnityTargetGuid = GetUnityFrameworkTargetGuid();
        }

        /// <summary>
        /// Recursively adds all files starting at a directory to the Test Target, and removes them from the Unity Target
        /// </summary>
        /// <param name="directory">Directory where the files are located</param>
        /// <exception cref="DirectoryNotFoundException">Given directory does not have a valid path</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public void SetFilesInDirectoryToTestTarget(DirectoryInfo directory) {
            try {
                foreach (var file in directory.GetFiles()) {
                    // Removes the build path from the absolute path and removes the first backslash
                    var localFilePath = file.ToString().Replace(BuildPath, "").Substring(1);
                    string fileGuid = FindFileGuidByProjectPath(localFilePath);
                    AddFileToBuild(TestTargetGuid, fileGuid);
                    RemoveFileFromBuild(UnityTargetGuid, fileGuid);
                }

                foreach (var otherDirectory in directory.GetDirectories()) {
                    SetFilesInDirectoryToTestTarget(otherDirectory);
                }
            } catch (Exception) {
                throw;
            }
        }

        /// <summary>
        /// Adds the single file to the test target.
        /// </summary>
        /// <param name="localFilepath">Local filepath to the file to add to test target.</param>
        /// <exception cref="DirectoryNotFoundException">Given directory does not have a valid path</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        public void AddFileToMainTarget(string filepath) {
            var localFilepath = filepath.Replace(BuildPath, "").Substring(1);
            string fileGuid = FindFileGuidByProjectPath(localFilepath);
            AddFileToBuild(UnityTargetGuid, fileGuid);
        }

        /// <summary>
        /// Returns an array of all the Target GUIDs
        /// </summary>
        public string[] GetAllTargetGuids() {
            return new string[] { TestTargetGuid, UnityTargetGuid };
        }

        /// <summary>
        /// Returns the Debug configuration for a Target
        /// </summary>
        /// <param name="targetGuid">GUID of the Target</param>
        public string GetDebugConfig(string targetGuid) {
            return BuildConfigByName(targetGuid, "Debug");
        }

        /// <summary>
        /// Overwrites current project on disk with modified configurations
        /// </summary>
        public void Save() {
            WriteToFile(PBXProject.GetPBXProjectPath(BuildPath));
        }
    }
}
#endif
