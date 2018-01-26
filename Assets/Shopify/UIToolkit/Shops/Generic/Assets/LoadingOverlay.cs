namespace Shopify.UIToolkit.Shops {
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class LoadingOverlay : MonoBehaviour {
        private const float AnimationSpeed = 4.0f;
        /// <summary>
        /// The Image that blocks the unloaded content underneath the overlay.
        /// This image is faded in/out depending on if the loading overlay is currently shown.
        /// </summary>
        public Image Blocker;
        private Image[] _placeholders;

        private bool _shown;

        private void Start() {
            _placeholders = GetComponentsInChildren<Image>().Where((x) => x != Blocker).ToArray();
            Show();
        }

        /// <summary>
        /// Show the loading overlay
        /// </summary>
        public void Show() {
            gameObject.SetActive(true);
            Blocker.CrossFadeAlpha(1.0f, 0.5f, true);
            StartCoroutine(AnimatePlaceholders());
        }

        /// <summary>
        /// Hide the loading overlay
        /// </summary>
        public void Hide() {
            StartCoroutine(HideRoutine());
        }

        private IEnumerator HideRoutine() {
            Blocker.CrossFadeAlpha(0.0f, 0.5f, true);

            foreach (var placeholder in _placeholders) {
                placeholder.CrossFadeAlpha(0, 0.25f, true);
            }

            yield return new WaitForSecondsRealtime(0.5f);
            gameObject.SetActive(false);
        }

        private IEnumerator AnimatePlaceholders() {
            var elapsed = 0f;
            var fadeInDuration = 0.5f;

            while (isActiveAndEnabled) {
                var fadeInFactor = Mathf.Clamp01(elapsed / fadeInDuration);

                for (int i = 0; i < _placeholders.Length; i++) {
                    var color = _placeholders[i].color;
                    color.a = Mathf.Clamp(
                        Mathf.Sin(
                            ((Time.time * AnimationSpeed) + ((float)i / _placeholders.Length) * Mathf.PI)
                        )/2f + 1.0f, 
                        0f, 
                        1.0f
                    ) * fadeInFactor;
                    _placeholders[i].color = color;
                }

                yield return null;
                elapsed += Time.deltaTime;
            }
        }
    }
}