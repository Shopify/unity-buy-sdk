namespace Shopify.UIToolkit.Shops.Generic
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Spinner : MonoBehaviour {
        public float RotationsPerSecond;
        public void Update() {
            transform.rotation = Quaternion.Euler(0, 0, -Time.time * RotationsPerSecond * 360);
        }
    }

}
