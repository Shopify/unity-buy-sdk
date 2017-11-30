#if !SHOPIFY_MONO_UNIT_TEST
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

        public static T AddComponent<T>() where T : Component {
            if (GameObject == null) {
                GameObject = new GameObject("Shopify");
            }

            return GameObject.AddComponent<T>();
        }
    }
}
#endif