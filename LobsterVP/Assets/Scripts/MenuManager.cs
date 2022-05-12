using UnityEngine;
using System.Collections;
using SimpleFileBrowser;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MenuManager : MonoBehaviour {
	[SerializeField] LobsterVideoPlayer lvp;
	[SerializeField] TMP_InputField joinField; 
	[SerializeField] TextMeshProUGUI createLabel; 
	[SerializeField] Button playButton;

	bool partyReady = false;

	void Start() {
		lvp.gameObject.SetActive(false);
	}

	void Update() {
		if (partyReady) playButton.enabled = true;
		else playButton.enabled = false;
	}

	public void JoinButton() {
		string code = joinField.text;
		createLabel.text = code;
	}

	public void CreateButton() {
		int length = 5;
	    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
	    string code = new string(Enumerable.Repeat(chars, length)
			.Select(s => s[Random.Range(0, s.Length)]).ToArray());

		createLabel.text = code;
	}

	public void LoadVideoButton() {
		FileBrowser.SetFilters(true, new FileBrowser.Filter( "Videos", ".mp4", ".mkv" ));
		FileBrowser.SetDefaultFilter("Videos");

		StartCoroutine(ShowLoadDialogCoroutine());
	}

	public void PlayVideoButton() {
		lvp.InitializeAndPlay();
	}

	IEnumerator ShowLoadDialogCoroutine() {
		yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Load Video", "Load");

		if (FileBrowser.Success) {
			lvp.SetVideoURL(FileBrowser.Result[0]);
			partyReady = true;
		}
	}
}
