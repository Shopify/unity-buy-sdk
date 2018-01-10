namespace Shopify.UIToolkit {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ViewSwitcher : MonoBehaviour {
        public const float AnimationDuration = 0.3f;

        public RectTransform Container;
        public RectTransform[] Views;

        private Stack<RectTransform> _viewStack = new Stack<RectTransform>();

        public void Start() {
            _viewStack.Push(Views[0]);

            for (int i = 1; i < Views.Length; i++) {
                Views[i].gameObject.SetActive(false);
            }
        }

        public void PushView(RectTransform view) {
            if (!System.Array.Exists(Views, (x) => x == view)) {
                throw new System.ArgumentException("View was not registered with ViewSwitcher before being pushed");
            }

            if (view.parent != Container) {
                throw new System.ArgumentException("View was not a child of the ViewSwitcher's container transform");
            }

            if (_viewStack.Peek() == view) return;

            _viewStack.Push(view);
            view.gameObject.SetActive(true);

            StartCoroutine(AnimateInView(view));
        }

        public void GoBack() {
            if (!CanNavigateBack()) return;
            var view = _viewStack.Pop();
            var nextView = _viewStack.Peek();
            StartCoroutine(AnimateOutView(view, nextView));
        }

        public bool CanNavigateBack() {
            return _viewStack.Count > 1;
        }

        private IEnumerator AnimateInView(RectTransform view) {
            view.anchoredPosition = Vector3.down * Container.rect.height;
            view.SetAsLastSibling();

            var elapsed = 0f;

            var animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            while (elapsed < AnimationDuration) {
                elapsed += Time.deltaTime;

                view.anchoredPosition = Vector3.LerpUnclamped(
                    Vector3.down * Container.rect.height,
                    Vector3.zero,
                    animationCurve.Evaluate(elapsed / AnimationDuration)
                );

                yield return null;
            }

            view.anchoredPosition = Vector3.zero;
        }

        private IEnumerator AnimateOutView(RectTransform view, RectTransform nextView) {
            nextView.gameObject.SetActive(true);

            nextView.anchoredPosition = Vector3.zero;
            view.anchoredPosition = Vector3.zero;

            nextView.SetAsLastSibling();
            view.SetAsLastSibling();

            var elapsed = 0f;

            var animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            while (elapsed < AnimationDuration) {
                elapsed += Time.deltaTime;

                view.anchoredPosition = Vector3.LerpUnclamped(
                    Vector3.zero,
                    Vector3.down * Container.rect.height,
                    animationCurve.Evaluate(elapsed / AnimationDuration)
                );

                yield return null;
            }

            if (!_viewStack.Contains(view)) {
                view.gameObject.SetActive(false);
            }
        }
    }
}