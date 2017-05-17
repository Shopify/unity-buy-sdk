namespace Shopify.Unity {
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Callbacks;

    public class PostProcesser {

        [PostProcessBuildAttribute(0)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
            if (target == BuildTarget.iOS) {
                iOSPostProcessor.Process(pathToBuiltProject);
            }
        }
    }
}