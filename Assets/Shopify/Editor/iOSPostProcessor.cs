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

            public void Save() {
                project.WriteToFile(PBXProject.GetPBXProjectPath(buildPath));
            }
        }

        public static void Process(string buildPath) {
            string testPath   = Path.Combine(buildPath, iOSTestsPath);
            var testDirectory = new DirectoryInfo(testPath);
            var projectInfo   = new ProjectInfo(buildPath);    
            projectInfo.SetFilesInDirectoryToTestTarget(testDirectory);
            projectInfo.Save();
        }
    }
}