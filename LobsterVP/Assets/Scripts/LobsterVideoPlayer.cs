// TODO: make the volume slider work
// TODO: show video progress in seconds
// TODO: make main menu attractive
// TODO: show party size
// TOOD: autoload last + resume at time
// TODO: chat
// TODO: mobile version

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
    bool muted;

    bool knobIsDragging;
    bool videoIsJumping;
    bool playing;
    VideoPlayer videoPlayer;

    float volume;
    
    void Awake() {
        view = PhotonView.Get(this);
        videoPlayer = GetComponent<VideoPlayer>();

        SetVolume(PlayerPrefs.GetFloat("volume", 0.5f));
    }

    void Update() {
        if (!knobIsDragging && !videoIsJumping) {
            if (videoPlayer.frameCount > 0) {
                float progress = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
                progressSlider.value = progress;
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.K)) {
                if (playing) {
		            view.RPC("PlayForAll", RpcTarget.All);
				} else {
		            view.RPC("PauseForAll", RpcTarget.All);
				}
			}

            if (Input.GetKeyDown(KeyCode.M)) {
                muted = !muted;
                videoPlayer.SetDirectAudioMute(0, muted);
			}

            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                SetVolume(volume + 0.1f);
			}

            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                SetVolume(volume - 0.1f);
			}

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.J)) {
                // reverse ten sec
                float percentChange = 10f / (float)videoPlayer.length;
		        view.RPC("SetTimeForAll", RpcTarget.All, Mathf.Clamp01(progressSlider.value - percentChange));
			}

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.L)) {
                // forward ten sec
                float percentChange = 10f / (float)videoPlayer.length;
		        view.RPC("SetTimeForAll", RpcTarget.All, Mathf.Clamp01(progressSlider.value + percentChange));
			}
        }
    }

    public void SetVideoURL(string url) {
		videoPlayer.url = url;
        videoPlayer.frame = 0;
	}

    public void InitializeAndPlay() {
        gameObject.SetActive(true);

        if (PhotonNetwork.IsMasterClient) {
		    view.RPC("SetTimeForAll", RpcTarget.All, progressSlider.value);
		    view.RPC("PlayForAll", RpcTarget.All);
		}
	}

    void SetVolume(float v) {
        volume = Mathf.Clamp01(v);
        videoPlayer.SetDirectAudioVolume(0, volume);
	}

    void OpenOptions() { }
    void CloseOptions() { }

	#region BUTTON FUNCS
	public void PlayVideo() {
		view.RPC("PlayForAll", RpcTarget.All);
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
        view.RPC("SetTimeForAll", RpcTarget.All, progressSlider.value);
		view.RPC("PlayForAll", RpcTarget.All);
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

    IEnumerator DelayedSetVideoIsJumpingToFalse() { // TOOD: try removing this and see if anything changes
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
	void PlayForAll() {
        videoPlayer.Play();
        playing = true;
        pauseButton.SetActive(true);
        playButton.SetActive(false);
	}

	[PunRPC]
    void SetTimeForAll(float t) {
        progressSlider.value = t;
        VideoJump();
        StartCoroutine(DelayedSetVideoIsJumpingToFalse());
	}

	[PunRPC]
    void PauseForAll() {
        videoPlayer.Pause();
        playing = false;
        pauseButton.SetActive(false);
        playButton.SetActive(true);
	}
	#endregion 
}
