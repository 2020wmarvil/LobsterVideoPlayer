using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;

public class LobsterVideoPlayer : MonoBehaviour {
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject enterFSButton;
    [SerializeField] GameObject exitFSButton;

    [SerializeField] Slider progressSlider;
    [SerializeField] RectTransform progressSliderBG;

    PhotonView view;

    bool optionsOpen = false;

    bool knobIsDragging;
    bool videoIsJumping;
    VideoPlayer videoPlayer;
    
    void Awake() {
        view = PhotonView.Get(this);
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Update() {
        if (!knobIsDragging && !videoIsJumping) {
            if (videoPlayer.frameCount > 0) {
                float progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
                progressSlider.value = progress;
            }
        }
    }

    public void SetVideoURL(string url) {
		videoPlayer.url = url;
        videoPlayer.frame = 0;
	}

    public void InitializeAndPlay() {
        gameObject.SetActive(true);
        PlayVideo(0f);
	}

    void OpenOptions() { }
    void CloseOptions() { }

	#region BUTTON FUNCS
    public void PlayVideoButton() {
		view.RPC("PlayForAll", RpcTarget.All, progressSlider.value);
	}

	public void PlayVideo(float t) {
        progressSlider.value = t;

        videoPlayer.Play();
        pauseButton.SetActive(true);
        playButton.SetActive(false);

        VideoJump();
        StartCoroutine(DelayedSetVideoIsJumpingToFalse());
	}

    public void PauseVideo() {
		view.RPC("PauseForAll", RpcTarget.All);
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
	#endregion

	#region KNOB
	public void KnobOnPressDown() {
        PauseVideo();
    }

    public void KnobOnRelease() {
        knobIsDragging = false;
		view.RPC("PlayForAll", RpcTarget.All, progressSlider.value);
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

	#region NETWORKING

	[PunRPC]
	void PlayForAll(float t) {
        PlayVideo(t);
	}

	[PunRPC]
    void PauseForAll() {
        videoPlayer.Pause();
        pauseButton.SetActive(false);
        playButton.SetActive(true);
	}
	#endregion 
}
