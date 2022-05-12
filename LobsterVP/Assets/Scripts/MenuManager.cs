using UnityEngine;
using System.Collections;
using SimpleFileBrowser;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

public class MenuManager : MonoBehaviourPunCallbacks {
	[SerializeField] LobsterVideoPlayer lvp;
	[SerializeField] TMP_InputField joinField; 
	[SerializeField] TextMeshProUGUI createLabel; 
	[SerializeField] Button playButton;

	bool partyReady = false;

	void Awake() {
		PhotonNetwork.ConnectUsingSettings();
	}

	void Start() {
		lvp.gameObject.SetActive(false);
	}

	void Update() {
		if (partyReady) playButton.enabled = true;
		else playButton.enabled = false;
	}

	public void JoinButton() {
		string roomName = joinField.text;
		createLabel.text = roomName;

		JoinRoom(roomName);
	}

	public void CreateButton() {
		int length = 5;
	    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
	    string roomName = new string(Enumerable.Repeat(chars, length)
			.Select(s => s[Random.Range(0, s.Length)]).ToArray());

		createLabel.text = roomName;
		GUIUtility.systemCopyBuffer = roomName;

		CreateRoom(roomName);
	}

	public void LoadVideoButton() {
		FileBrowser.SetFilters(true, new FileBrowser.Filter( "Videos", ".mp4", ".mkv" ));
		FileBrowser.SetDefaultFilter("Videos");

		StartCoroutine(ShowLoadDialogCoroutine());
	}

	public void PlayVideoButton() {
		PhotonView photonView = PhotonView.Get(this);
		photonView.RPC("PlayVideoForAll", RpcTarget.All);
	}

	[PunRPC]
	void PlayVideoForAll() {
		lvp.InitializeAndPlay();
	}

	IEnumerator ShowLoadDialogCoroutine() {
		yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Load Video", "Load");

		if (FileBrowser.Success) {
			lvp.SetVideoURL(FileBrowser.Result[0]);
			partyReady = true;
		}
	}

	#region NETWORKING
	// JoinRandom??  PhotonNetwork.JoinRandomRoom(); 

	void JoinRoom(string roomName) {
		PhotonNetwork.JoinRoom(roomName);
	}

	void CreateRoom(string roomName) {
		PhotonNetwork.CreateRoom(roomName);
	}

	public override void OnPlayerEnteredRoom(Player other) {
		print("joined");
	}

	public override void OnConnectedToMaster() {
		print("Conneced to master");
	}
	#endregion
}
