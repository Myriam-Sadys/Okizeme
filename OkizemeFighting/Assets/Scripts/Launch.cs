﻿    // --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to connect, and join/create room automatically
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

/// <summary>
/// Launch manager. Connect, join a random room or create one if none or all full.
/// </summary>
public class Launch : Photon.PunBehaviour {

	#region Public Variables

	[Tooltip("The Ui Panel to let the user enter name, connect and play")]
	public GameObject controlPanel;

	[Tooltip("The Ui Text to inform the user about the connection progress")]
	public Text feedbackText;

	[Tooltip("The maximum number of players per room")]
	public byte maxPlayersPerRoom = 2;

	[Tooltip("The name of the first player")]
	public string NamePlayer1 = "Merwan";

	[Tooltip("The name of the second player")]
	public string NamePlayer2 = "Seb";

    [Tooltip("The number of seconds before starting the game")]
    public float timeLeft = 3.0f;

    [Tooltip("The UI Loader Anime")]
    public LoaderAnime loaderAnime;

    #endregion

    #region Private Variables
    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

	/// <summary>
	/// This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).
	/// </summary>
	string _gameVersion = "1";

    private bool stop = true;
    private float minutes;
    private float seconds;



    #endregion

    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
	{
		// #Critical
		// we don't join the lobby. There is no need to join a lobby to get the list of rooms.
		PhotonNetwork.autoJoinLobby = false;

		// #Critical
		// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
		PhotonNetwork.automaticallySyncScene = true;

    }

    void Update()
    {
        if (stop) return;
        timeLeft -= Time.deltaTime;

        minutes = Mathf.Floor(timeLeft / 60);
        seconds = timeLeft % 60;
        if (seconds > 59) seconds = 59;
        if (minutes < 0)
        {
            stop = true;
            minutes = 0;
            seconds = 0;
        }
    }


    #endregion


    #region Public Methods

    /// <summary>
    /// Start the connection process. 
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
	{
		// we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
		feedbackText.text = "";

		// keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
		isConnecting = true;

		// hide the Play button for visual consistency
		controlPanel.transform.GetChild(0).gameObject.SetActive(false);

        // start the loader animation for visual effect.
        if (loaderAnime != null)
        {
            loaderAnime.StartLoaderAnimation();
        }

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.connected)
		{
			LogFeedback("Joining Room...");
			// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
			PhotonNetwork.JoinRandomRoom();
		}else{

			LogFeedback("Connecting...");
				
			// #Critical, we must first and foremost connect to Photon Online Server.
			PhotonNetwork.ConnectUsingSettings(_gameVersion);
		}
	}

	/// <summary>
	/// Logs the feedback in the UI view for the player, as opposed to inside the Unity Editor for the developer.
	/// </summary>
	/// <param name="message">Message.</param>
	void LogFeedback(string message)
	{
		// we do not assume there is a feedbackText defined.
		if (feedbackText == null) {
			return;
		}

		// add new messages as a new line and at the bottom of the log.
		feedbackText.text += System.Environment.NewLine+message;
	}

	#endregion


	#region Photon.PunBehaviour CallBacks
	// below, we implement some callbacks of PUN
	// you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage


	/// <summary>
	/// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false.
	/// </summary>
	public override void OnConnectedToMaster()
	{

		Debug.Log("Region:"+PhotonNetwork.networkingPeer.CloudRegion);

		// we don't want to do anything if we are not attempting to join a room. 
		// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
		// we don't want to do anything.
		if (isConnecting)
		{
			LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
			Debug.Log("Animator/Launch: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");
		
			// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
			PhotonNetwork.JoinRandomRoom();
		}
	}

	/// <summary>
	/// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
	/// </summary>
	/// <remarks>
	/// Most likely all rooms are full or no rooms are available. <br/>
	/// </remarks>
	/// <param name="codeAndMsg">codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg.</param>
	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
	{
		LogFeedback("<Color=Red>OnPhotonRandomJoinFailed</Color>: Next -> Create a new Room");
		Debug.Log("Animator/Launch:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 2}, null);");

		// #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
		PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = this.maxPlayersPerRoom}, null);
	}


	/// <summary>
	/// Called after disconnecting from the Photon server.
	/// </summary>
	/// <remarks>
	/// In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
	/// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().
	/// </remarks>
	public override void OnDisconnectedFromPhoton()
	{
		LogFeedback("<Color=Red>OnDisconnectedFromPhoton</Color>");
		Debug.LogError("Animator/Launcher:Disconnected");

        // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
        loaderAnime.StopLoaderAnimation();

        isConnecting = false;
        controlPanel.transform.GetChild(0).gameObject.SetActive(false);

    }

	/// <summary>
	/// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
	/// </summary>
	/// <remarks>
	/// This method is commonly used to instantiate player characters.
	/// If a match has to be started "actively", you can call an [PunRPC](@ref PhotonView.RPC) triggered by a user's button-press or a timer.
	///
	/// When this is called, you can usually already access the existing players in the room via PhotonNetwork.playerList.
	/// Also, all custom properties should be already available as Room.customProperties. Check Room..PlayerCount to find out if
	/// enough players are in the room to start playing.
	/// </remarks>
	public override void OnJoinedRoom()
	{
		LogFeedback("<Color=Green>OnJoinedRoom</Color> with "+PhotonNetwork.room.PlayerCount+" Player(s)");
		Debug.Log("Animator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");

        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.automaticallySyncScene to sync our instance scene.
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.playerName = NamePlayer1;
        else
            PhotonNetwork.playerName = NamePlayer2;
        PlayerPrefs.SetString("PlayerName", PhotonNetwork.playerName);
        Debug.Log(PhotonNetwork.playerName);
        if (PhotonNetwork.room.PlayerCount == maxPlayersPerRoom)
        {
            PhotonView PV = PhotonView.Get(this);
            PV.RPC("JoinRoom", PhotonTargets.All);
        }
    }

    [PunRPC]
    private void JoinRoom()
    {
        stop = false;
        timeLeft = 3f;
        Update();
        StartCoroutine(updateCoroutine());
    }

    private IEnumerator updateCoroutine()
    {
        while (!stop)
        {
            feedbackText.text = string.Format("{0:0}", seconds);
            yield return new WaitForSeconds(0.2f);
        }
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.LoadLevel("MainScene2");
    }

    #endregion

}