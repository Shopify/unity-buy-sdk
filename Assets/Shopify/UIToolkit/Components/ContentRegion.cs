using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ScrollEvent : UnityEvent<bool, bool>
{
}

public class ContentRegion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public ScrollEvent OnScroll = new ScrollEvent();

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
			bool clipStart = false;
			bool clipEnd = false;

            Vector3 currentPosition = Input.mousePosition;

            Vector3 diff = currentPosition - startMousePosition;
            float newX = startPosition.x + diff.x;

            if (newX > 0) {
                newX = 0;
				clipStart = true;
            }

            if (newX < endOffset) {
                newX = endOffset;
				clipEnd = true;
            }

            Vector3 pos = new Vector3(newX, 0, 0);

            transform.localPosition = pos;

			OnScroll.Invoke (clipStart, clipEnd);
        }
    }
}
