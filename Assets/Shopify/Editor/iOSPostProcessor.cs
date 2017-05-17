namespace Shopify.Unity {
    using UnityEngine;
    using UnityEditor.iOS.Xcode;
    using System.IO;

    public class iOSPostProcessor {
        
        const string iOSTestsPath = "Libraries/Plugins/iOS/Shopify/BuyTests/";

        private struct ProjectInfo {

            public PBXProject project;
            public string buildPath;
            public string testTargetID;
            public string unityTargetID;

            public ProjectInfo(string buildPath) {
                this.buildPath = buildPath;
                project        = new PBXProject();
                project.ReadFromFile(PBXProject.GetPBXProjectPath(buildPath));
                testTargetID   = project.TargetGuidByName(PBXProject.GetUnityTestTargetName());
                unityTargetID  = project.TargetGuidByName(PBXProject.GetUnityTargetName());
            }

            public void SetFilesInDirectoryToTestTarget(DirectoryInfo directory) {
                
                // Change files at current directory to test target
                // Prior to PostProcessing all files are part of the Unity Target
                foreach (var file in directory.GetFiles()) {
                    var localFilePath = file.ToString().Replace(buildPath, "").Substring(1);
                    string fileID     = project.FindFileGuidByProjectPath(localFilePath);
                    project.AddFileToBuild(testTargetID, fileID);
                    project.RemoveFileFromBuild(unityTargetID, fileID);
                }

                // Recurse through other directories
                foreach (var otherDirectory in directory.GetDirectories()) {
                    SetFilesInDirectoryToTestTarget(otherDirectory);
                }
            }

            public string[] GetAllTargetIds() {
                return new string[] {testTargetID, unityTargetID};
            }

            public void Save() {
                project.WriteToFile(PBXProject.GetPBXProjectPath(buildPath));
            }
        }

        public static void Process(string buildPath) {
            string testPath   = Path.Combine(buildPath, iOSTestsPath);
            var testDirectory = new DirectoryInfo(testPath);
            var info          = new ProjectInfo(buildPath);    
            info.SetFilesInDirectoryToTestTarget(testDirectory);
            info.project.SetBuildProperty(info.GetAllTargetIds(), "SWIFT_VERSION",           "3.0");
            info.project.SetBuildProperty(info.unityTargetID,     "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            info.project.SetBuildProperty(info.testTargetID,      "LD_RUNPATH_SEARCH_PATHS", "@loader_path/Frameworks");
            info.project.SetBuildProperty(info.testTargetID,      "PRODUCT_MODULE_NAME",     "$(PRODUCT_NAME:c99extidentifier)Tests");
            info.Save();
        }
    }
}