using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Panels {
    public class EmptyCartPanel : MonoBehaviour {

        public Button BackToProductsButton;
        public Button ContinueButton;

        public UnityEvent OnReturnToProducts;

        private void Start() {
            gameObject.SetActive(false);

            BackToProductsButton.onClick.AddListener(ReturnToProducts);
            ContinueButton.onClick.AddListener(ReturnToProducts);
        }

        private void ReturnToProducts() {
            OnReturnToProducts.Invoke ();
		}
	}
}
