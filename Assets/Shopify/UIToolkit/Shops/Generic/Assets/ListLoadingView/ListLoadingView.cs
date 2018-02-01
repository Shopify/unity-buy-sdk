using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListLoadingView : MonoBehaviour {
	public Animator[] CellAnimators;
	public float TimingOffset;

	void Start () {
		StartCoroutine(StartAnimations());
	}

	IEnumerator StartAnimations() {
		foreach (var animator in CellAnimators) {
			animator.Play("Fading");
			yield return new WaitForSeconds(TimingOffset);
		}
	}
}
