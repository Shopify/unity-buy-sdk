using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components {
    public class RenderDepthTexture : MonoBehaviour {
        private void Start() {
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        }
    }
}