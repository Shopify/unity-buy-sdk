using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationCanvas : MonoBehaviour {
    public UnityEvent OnAnimationStarted;
    public UnityEvent OnAnimationStopped;

    public void AnimationStarted() {
        OnAnimationStarted.Invoke ();
    }

    public void AnimationStopped() {
        OnAnimationStopped.Invoke ();
    }
}
