namespace Shopify.Unity.SDK {
    using UnityEngine;

    public class GlobalGameObject {
        private static GameObject GameObject;

        public static string Name {
            get {
                if (GameObject != null) {
                    return GameObject.name;
                } else {
                    return null;
                }
            }
        }

        public static void Destroy() {
            MonoBehaviour.DestroyImmediate(GameObject);
        }

        public static T AddComponent<T>() where T: Component {
            if (GameObject == null) {
                GameObject = new GameObject("Shopify");
                GameObject.DontDestroyOnLoad(GameObject);
            }

            return GameObject.AddComponent<T>();
        }
    }
}
