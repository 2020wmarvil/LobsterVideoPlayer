using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.UI;

public class LobsterVideoPlayer : MonoBehaviour {
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject enterFSButton;
    [SerializeField] GameObject exitFSButton;

    [SerializeField] Slider progressSlider;
    [SerializeField] RectTransform progressSliderBG;

    bool optionsOpen = false;

    bool knobIsDragging;
    bool videoIsJumping;
    VideoPlayer videoPlayer;
    
    void Start() {
        videoPlayer = GetComponent<VideoPlayer>();
        PlayVideo();
        pauseButton.SetActive(true);
        playButton.SetActive(false);
    }

    void Update() {
        if (!knobIsDragging && !videoIsJumping) {
            if (videoPlayer.frameCount > 0) {
                float progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
                progressSlider.value = progress;
            }
        }
    }

    void OpenOptions() { }
    void CloseOptions() { }

	#region BUTTON FUNCS
	public void PlayVideo() {
        videoPlayer.Play();
        pauseButton.SetActive(true);
        playButton.SetActive(false);
	}

    public void PauseVideo() {
        videoPlayer.Pause();
        pauseButton.SetActive(false);
        playButton.SetActive(true);
    }

	public void Options() {
        if (optionsOpen) OpenOptions();
        else CloseOptions();
    }

	public void EnterFullScreen() {
        Screen.fullScreen = true;
        enterFSButton.SetActive(false);
        exitFSButton.SetActive(true);
	}

	public void ExitFullScreen() {
        Screen.fullScreen = false;
        enterFSButton.SetActive(true);
        exitFSButton.SetActive(false);
	}

    public void OnSlider(float t) {
        videoPlayer.time = t * videoPlayer.length;
	}
	#endregion

	#region KNOB
	public void KnobOnPressDown() {
        PauseVideo();
    }

    public void KnobOnRelease() {
        knobIsDragging = false;
        PlayVideo();
        VideoJump();
        StartCoroutine(DelayedSetVideoIsJumpingToFalse());
    }

    public void KnobOnDrag() {
        knobIsDragging = true;
        videoIsJumping = true;

        float screenPosX = new Vector2(Input.mousePosition.x, Input.mousePosition.y).x;
        Vector2 size = Vector2.Scale(progressSliderBG.rect.size, progressSliderBG.lossyScale);
        Rect r = new Rect((Vector2)progressSliderBG.position - (size * 0.5f), size);
        float t = Mathf.Clamp01((screenPosX - r.xMin) / (r.xMax - r.xMin));
        progressSlider.value = t;
    }

    IEnumerator DelayedSetVideoIsJumpingToFalse() {
        yield return new WaitForSeconds(2);
        videoIsJumping = false;
    }

    void VideoJump() {
        var frame = videoPlayer.frameCount * progressSlider.value;
        videoPlayer.frame = (long)frame;
    }
	#endregion
}
