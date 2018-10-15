namespace Shopify.Unity.SDK {
    using System.Collections;
    using System.Text;
    using System;
    using UnityEngine;

    internal class TimeoutComponent : MonoBehaviour {
        public void StartTimeout(float duration, Action callback) {
            StartCoroutine(DoTimeout(duration, callback));
        }

        private IEnumerator DoTimeout(float duration, Action callback) {
            yield return new WaitForSeconds(duration);

            callback();
        }
    }

    public class UnityTimeout {
        private static TimeoutComponent component;

        public static void Start(float duration, Action callback) {
            if (component == null) {
                component = GlobalGameObject.AddComponent<TimeoutComponent>();
            }

            component.StartTimeout(duration, callback);
        }
    }
}
