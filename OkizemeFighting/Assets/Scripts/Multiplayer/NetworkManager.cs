using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SO;
using System.Linq;

namespace SA
{
	public class NetworkManager : Photon.PunBehaviour
	{
		public static bool isMaster;
		public static NetworkManager singleton;

		List<MultiplayerHolder> multiplayerHolders = new List<MultiplayerHolder>();
		public MultiplayerHolder GetHolder(int photonId)
		{
			for (int i = 0; i < multiplayerHolders.Count; i++)
			{
				if (multiplayerHolders[i].ownerId == photonId)
				{
					return multiplayerHolders[i];
				}
			}

			return null;
		}

		public Card GetCard(int instId, int ownerId)
		{
			MultiplayerHolder h = GetHolder(ownerId);
			return h.GetCard(instId);
		}

		ResourcesManager rm;
		int cardInstIds;

		public StringVariable logger;
		public GameEvent loggerUpdated;
		public GameEvent failedToConnect;
		public GameEvent onConnected;
		public GameEvent waitingForPlayer;

		private void Awake()
		{
			if (singleton == null)
			{
				rm = Resources.Load("ResourcesManager") as ResourcesManager;
				singleton = this;
				DontDestroyOnLoad(this.gameObject);
			}
			else {
				Destroy(this.gameObject);
			}
		}

		private void Start()
		{
			rm.Init();
			PhotonNetwork.autoCleanUpPlayerObjects = false;
			PhotonNetwork.autoJoinLobby = false;
			PhotonNetwork.automaticallySyncScene = false;
			Init();
		}


		public void Init()
		{
			PhotonNetwork.ConnectUsingSettings("1");
			logger.value = "Connecting";
			loggerUpdated.Raise();
		}

		#region My Calls
		public void OnPlayGame()
		{
			JoinRandomRoom();
		}

		void JoinRandomRoom()
		{
			PhotonNetwork.JoinRandomRoom();
		}

		void CreateRoom()
		{
			RoomOptions room = new RoomOptions();
			room.MaxPlayers = 2;
			PhotonNetwork.CreateRoom(RandomString(256), room, TypedLobby.Default);
		}

		private System.Random random = new System.Random();
		public string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefgolkip";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}


		//Master only
		public void PlayerJoined(int photonId, string[] cards)
		{
			MultiplayerHolder m = new MultiplayerHolder();
			m.ownerId = photonId;

			for (int i = 0; i < cards.Length-1; i++)
			{
				Card c = CreateCardMaster(cards[i]);
				if (c == null)
					continue;

				m.RegisterCard(c);
				//RPC
			}
		}

		Card CreateCardMaster(string cardId)
		{
			Card card = rm.GetCardInstance(cardId);
			card.instId = cardInstIds;
			cardInstIds++;

			return card;
		}

		//void CreateCardClient_call(string cardId, int instId, int photonId)
		//{
		//	Card c = CreateCardClient(cardId, instId);
		//	if (c != null)
		//	{
		//		MultiplayerHolder h = GetHolder(photonId);
		//		h.RegisterCard(c);
		//	}
		//}

		//Card CreateCardClient(string cardId, int instId)
		//{
		//	Card card = rm.GetCardInstance(cardId);
		//	card.instId = instId;

		//	return card;
		//}

		#endregion

		#region Photon Callbacks
		public override void OnConnectedToMaster()
		{
			base.OnConnectedToMaster();
			logger.value = "Connected";
			loggerUpdated.Raise();
			onConnected.Raise();
		}

		public override void OnFailedToConnectToPhoton(DisconnectCause cause)
		{
			base.OnFailedToConnectToPhoton(cause);
			logger.value = "Failed To Connect";
			loggerUpdated.Raise();
			failedToConnect.Raise();
		}

		public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
		{
			base.OnPhotonRandomJoinFailed(codeAndMsg);
			CreateRoom();
		}

		public override void OnCreatedRoom()
		{
			base.OnCreatedRoom();
			isMaster = true;
		}

		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			logger.value = "Waiting for player";
			loggerUpdated.Raise();
			waitingForPlayer.Raise();
		}

		public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
		{
			if (isMaster)
			{
				if (PhotonNetwork.playerList.Length > 1)
				{
					logger.value = "Ready for match";
					loggerUpdated.Raise();

					PhotonNetwork.room.IsOpen = false;
					PhotonNetwork.Instantiate("MultiplayerManager", Vector3.zero, Quaternion.identity, 0);	
				}
			
			}
		}

		public void LoadGameScene()
		{
			SessionManager.singleton.LoadGameLevel(OnGameSceneLoaded);
		}

		void OnGameSceneLoaded()
		{
			MultiplayerManager.singleton.countPlayers = true;
		}

		public override void OnDisconnectedFromPhoton()
		{
			base.OnDisconnectedFromPhoton();
		}

		public override void OnLeftRoom()
		{
			base.OnLeftRoom();
		}
		#endregion

		#region RPCs
		#endregion
	}

	public class MultiplayerHolder {
		public int ownerId;
		Dictionary<int, Card> cards = new Dictionary<int, Card>();

		public void RegisterCard(Card c)
		{
			cards.Add(c.instId, c);
		}

		public Card GetCard(int instId)
		{
			Card r = null;
			cards.TryGetValue(instId, out r);
			return r;
		}
	}
}
