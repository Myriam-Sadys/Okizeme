using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Okizeme.Fight
{
    public class NetworkManagerPUN : Photon.PunBehaviour
    {

        #region Public Variables

        static public NetworkManagerPUN Instance;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        [Tooltip("The SpawnPoints of the players")]
        public GameObject SpawnPoint1;
        public GameObject SpawnPoint2;

        #endregion

        #region Private Variables

        private GameObject instance;
        private GameObject MyPlayer;

        #endregion

        #region MonoBehaviour CallBacks

        /// </summary>
        void Start()
        {
            Instance = this;

            // in case we started this demo with the wrong scene being active, simply load the menu scene
            if (!PhotonNetwork.connected)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> Not connected to photon.", this);
                SceneManager.LoadScene("WaitingForOpponent");
                return;
            }

            if (playerPrefab == null)
            { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

                Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.Log("We are Instantiating LocalPlayer from " + SceneManagerHelper.ActiveSceneName);

                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    if (PhotonNetwork.isMasterClient)
                        MyPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, SpawnPoint1.transform.position, Quaternion.identity, 0);
                    else
                        MyPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, SpawnPoint2.transform.position, Quaternion.identity, 0);
                    //MyPlayer.GetComponent<PlayerManager>().enabled = true;
                }
                else
                {
                    Debug.Log("Ignoring scene load for " + SceneManagerHelper.ActiveSceneName);
                }
            }

        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// </summary>
        void Update()
        {
            // "back" button of phone equals "Escape". quit app if that's pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApplication();
            }
        }

        #endregion

        #region Photon Messages

        /// <summary>
        /// Called when a Photon Player got connected. We need to then load a bigger scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected

                LoadArena();
            }
        }

        /// <summary>
        /// Called when a Photon Player got disconnected. We need to load a smaller scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected

                SceneManager.LoadScene("YouWon");
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("YouLost");
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        #endregion

        #region Private Methods

        void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }

            Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);

            PhotonNetwork.LoadLevel("FightScene");
        }

        #endregion

    }
}


















































    //public GameObject PlayerPrefab, SpawnPoint1, SpawnPoint2, HealthBar1, ZemeBar1, HealthBar2, ZemeBar2;
    //public static NetworkManagerPUN Instance;
    //public SA.Timer t; 
    //void Start()
    //{
    //    // On se connect directement au cloud de PUN, si vous avez sélectionné auto join dans la configuration
    //    // On devrait donc se connecter directement au Lobby
    //    Instance = this;
    //    PhotonNetwork.ConnectUsingSettings("tutopun");
    //}

    //// Après le Start() on rejoint le Lobby, c'est à dire qu'on est connecté au cloud de PUN
    //// Maintenant il faut rejoindre une Room
    //public void LeaveRoom()
    //{
    //    PhotonNetwork.LeaveRoom();
    //}

    //void OnJoinedLobby()
    //{
    //    // On va créer une room pour 2 personnes maximun
    //    RoomOptions MyRoomOptions = new RoomOptions();
    //    MyRoomOptions.MaxPlayers = 2;

    //    // Ici on set de manière aléatoire le nom de l'utilisateur pour aller plus vite
    //    PhotonNetwork.playerName = "Player" + UnityEngine.Random.Range(1, 500);

    //    // Enfin on rejoint la room demandé, du nom de "The Fight" mais on pourrait mettre un nom différent
    //    // afin de créer une autre room
    //    PhotonNetwork.JoinOrCreateRoom("The Fight", MyRoomOptions, TypedLobby.Default);
    //}

    //private static List<GameObject> GetObjectsInLayer(GameObject root, int layer)
    //{
    //    var ret = new List<GameObject>();
    //    foreach (Transform t in root.transform.GetComponentsInChildren(typeof(GameObject), true))
    //    {
    //        if (t.gameObject.layer == layer)
    //        {
    //            ret.Add(t.gameObject);
    //        }
    //    }
    //    return ret;
    //}
    //// Quand on a effectivement rejoint la room
    //void OnJoinedRoom()
    //{
    //    GameObject MyPlayer;
    //    //GameObject EnemyPlayer;
    //    // On instancie le joueur à tout le réseau
    //    // Nom du prefab à instancier / Position / Rotation / Groupe
    //    // Le groupe permet de différencier des joueurs, ou d'autre éléments, que vous vouliez différencier rapidement
    //    // les joueurs des adversaires OU différencier les équipes des joueurs
    //    if (PhotonNetwork.player.ID == 1)
    //    {
    //        MyPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPoint1.transform.position, Quaternion.identity, 0);
    //        MyPlayer.GetComponent<PlayerManager>().hb = HealthBar1.GetComponent<HealthBar>();
    //        MyPlayer.GetComponent<PlayerManager>().zb = ZemeBar1.GetComponent<ZemeBar>();
    //        Debug.Log("hey");

    //    }
    //    else
    //    {
    //        MyPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPoint2.transform.position, Quaternion.identity, 0);
    //        MyPlayer.GetComponent<PlayerManager>().hb = HealthBar2.GetComponent<HealthBar>();
    //        MyPlayer.GetComponent<PlayerManager>().zb = ZemeBar2.GetComponent<ZemeBar>();
    //        t.LaunchTimer();
    //        //MyPlayer.GetComponent<PlayerMoving>().Enemy = PhotonView.FindObjectOfType<PlayerMoving>();
    //        //PhotonView.FindObjectOfType<PlayerMoving>().Enemy = MyPlayer.GetComponent<PlayerMoving>().Enemy;
    //        //Debug.Log(MyPlayer.GetComponent<PlayerMoving>().Enemy);
    //    }
    //    // Le contrôle et la caméra sont désactivé afin d'être activé UNIQUEMENT pour le joueur en local
    //    // Et surtout pas contrôlé par les autres joueurs sur le réseau
    //    MyPlayer.GetComponent<PlayerManager>().enabled = true;
    //    //MyPlayer.GetComponentInChildren<Camera>().enabled = true;
    //    //MyPlayer.GetComponentInChildren<Camera>().GetComponent<AudioListener>().enabled = true;
    //}

    ////void OnPhotonInstantiate(PhotonMessageInfo info)
    ////{
    ////    // e.g. store this gameobject as this player's charater in Player.TagObject
    ////    info.sender.TagObject = this.GameObject;
    ////}
