#if UNITY_EDITOR
namespace Shopify.BuildPipeline {
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.IO;
    using UnityEngine;
    using UnityEditor;

    public class ShopifyBuildPipeline : MonoBehaviour {
        private const string PlayerPathArg = "-buildPlayerPath";
        private static string PlayerPath {
            get {
                string[] arguments = Environment.GetCommandLineArgs();
                string playerPath = ShopifyBuildPipeline.PlayerPathFromArguments(arguments);

                if (playerPath == null) {
                    playerPath = "./Shopify-iOS";
                }

                return Path.GetFullPath(playerPath);
            }
        }

        /// <summary>
        /// Build an iOS project that is ready to run Shopify tests on a Simulator
        /// </summary>
        [MenuItem("Build/Build Shopify iOS Tests")]
        public static void BuildIosForTests()
        {
            string path = PlayerPath;
            string[] scenes = {"Assets/Scenes/iOSTestScene.unity"};

            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
            BuildPipeline.BuildPlayer(scenes, path, BuildTarget.iOS, BuildOptions.Development);
            ShopifyIosPostProcessor.ProcessForTests(path);
        }

        private static string PlayerPathFromArguments(string[] arguments) {
            for (int i = 0; i < arguments.Length; i ++) {
                string arg = arguments[i];
                if (arg.Equals(PlayerPathArg) && i + 1 < arguments.Length) {
                    return arguments [i + 1];
                }
            }
            return null;
        }
    }
}
#endif
