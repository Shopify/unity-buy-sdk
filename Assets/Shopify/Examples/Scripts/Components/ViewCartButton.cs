using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Components {
    public class ViewCartButton : MonoBehaviour {

        public Text Quantity;
        public Image SingleDigitBadge;
        public Image DoubleDigitBadge;
        public Image CartIcon;

        public UnityEvent OnClick;

        private void Start() {
            gameObject.GetComponent<Button>().onClick.AddListener(() => OnClick.Invoke());
        }

        public void UpdateCartQuantity(int quantity) {
            Quantity.text = quantity.ToString();

            SingleDigitBadge.gameObject.SetActive(quantity < 10);
            DoubleDigitBadge.gameObject.SetActive(quantity >= 10);

            gameObject.SetActive(quantity > 0);
        }
    }
}
