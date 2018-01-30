using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ContentRegion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public UnityEvent OnScroll = new UnityEvent();

    private bool isMouseDown = false;
    private Vector3 startMousePosition;
    private Vector3 startPosition;

    public float endOffset;

    public void OnPointerDown(PointerEventData dt) {
        isMouseDown = true;

        startPosition = transform.localPosition;
        startMousePosition = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData dt) {
        isMouseDown = false;
    }

    void Update () {
        if (isMouseDown) {
            Vector3 currentPosition = Input.mousePosition;

            Vector3 diff = currentPosition - startMousePosition;
            float newX = Math.Max(Math.Min(startPosition.x + diff.x, 0), endOffset);

            Vector3 pos = new Vector3(newX, 0, 0);

            transform.localPosition = pos;

            OnScroll.Invoke();
        }
    }
}
