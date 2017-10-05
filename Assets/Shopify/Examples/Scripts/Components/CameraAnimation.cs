using UnityEngine;
using UnityEngine.UI;

namespace Components {
    public class CameraAnimation : MonoBehaviour {

        public Animator MainCameraAnimator;

        public void FocusOnVendingMachine() {
            MainCameraAnimator.SetBool("Focused", true);
        }

        public void Unfocus() {
            MainCameraAnimator.SetBool("Focused", false);
        }
    }
}
