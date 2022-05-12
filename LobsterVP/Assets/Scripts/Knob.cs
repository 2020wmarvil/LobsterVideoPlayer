using UnityEngine;
using UnityEngine.EventSystems;

public class Knob : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [SerializeField] GameObject videoPlayer;
    LobsterVideoPlayer vp;

	bool mouseDown;
	bool pointerLeft;
	bool pointerDown;

	void Awake() {
        vp = videoPlayer.GetComponent<LobsterVideoPlayer>();
    }

	void Update() {
		mouseDown = Input.GetMouseButton(0);

		if (!mouseDown && pointerLeft) {
			pointerLeft = false;
			pointerDown = false;
			vp.KnobOnRelease();
		} else if (mouseDown && pointerDown) {
			vp.KnobOnDrag();
		}
	}

	public void OnPointerDown(PointerEventData eventData) {
        vp.KnobOnPressDown();
		pointerDown = true;
	}

	public void OnPointerUp(PointerEventData eventData) {
		pointerLeft = true;
	}
}
