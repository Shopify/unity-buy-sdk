﻿namespace Shopify.UIToolkit {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A component that allows for switching between multiple UI "views",
    /// where a view is different UI that needs to be switched between modally.
    /// 
    /// In the Unity Buy SDK, we use this for the factory shops in order to switch
    /// between the cart/product list/product details views in the builtin multi
    /// product shops.
    /// </summary>
    public class ViewSwitcher : MonoBehaviour {
        /// <summary>
        /// The duration of the animation.
        /// </summary>
        public float AnimationDuration = 0.3f;

        /// <summary>
        /// The parent element that contains all of the views you want to switch between
        /// </summary>
        public RectTransform Container;

        /// <summary>
        /// A registry of views that you will want to switch between
        /// </summary>
        public RectTransform[] Views;

        private Stack<RectTransform> _viewStack = new Stack<RectTransform>();

        public void Start() {
            _viewStack.Push(Views[0]);

            for (int i = 1; i < Views.Length; i++) {
                Views[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Pushes a new view, making it the active view
        /// </summary>
        /// <param name="view">The rect transform of the UI that you want to push as a new view</param>
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

        /// <summary>
        /// Pops the last view off the stack and navigates back to the previously pushed view
        /// </summary>
        public void GoBack() {
            if (!CanNavigateBack()) return;
            var view = _viewStack.Pop();
            var nextView = _viewStack.Peek();
            StartCoroutine(AnimateOutView(view, nextView));
        }

        /// <summary>
        /// Are there enough views in the stack views to navigate back?
        /// </summary>
        /// <returns>True if there are enough views to navigate backwards</returns>
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