using System.Collections;
using UnityEngine;

public class VideoBarController : MonoBehaviour {
	[SerializeField] float displayTime = 2f;
	[SerializeField] GameObject videoBar;
	[SerializeField] float ySinkOffset;
	[SerializeField] float hideDuration = 0.5f;
	
	Vector2 lastMousePos;
	float lastTimeMouseMoved = 0f;
	bool hiding = false;

	Coroutine HideCoroutine;

	void Update() {
		Vector2 mousePos = Input.mousePosition;

		if (mousePos != lastMousePos) {
			lastTimeMouseMoved = Time.time;
			lastMousePos = mousePos;
		}

		bool shouldDisplay = Time.time - lastTimeMouseMoved < displayTime;

		if (shouldDisplay && hiding) {
			Show();
		} else if (!shouldDisplay && !hiding) {
			Hide();
		}
	}

	void Show() {
		hiding = false;
		StopCoroutine(HideCoroutine);
		videoBar.transform.localPosition = new Vector3(videoBar.transform.localPosition.x, -490f, videoBar.transform.localPosition.z);
		videoBar.SetActive(true);
	}

	void Hide() {
		hiding = true;
		HideCoroutine = StartCoroutine(HideImpl());
	}

	IEnumerator HideImpl() {
		float t = 0f;

		Vector3 basePos = videoBar.transform.position;
		Vector3 targetPos = basePos - new Vector3(0f, ySinkOffset, 0f);

		while (t < 1f) {
			t += Time.deltaTime / hideDuration;
			float progress = CubicEaseOut(t);

			videoBar.transform.position = Vector3.Lerp(basePos, targetPos, progress);
			yield return new WaitForEndOfFrame();
		}

		videoBar.SetActive(false);
	}

	float CubicEaseOut(float t) {
		return 1f - Mathf.Pow(1f - t, 3f);
	}
}
