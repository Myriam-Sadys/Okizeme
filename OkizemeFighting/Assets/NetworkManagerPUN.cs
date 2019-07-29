using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerPUN : MonoBehaviour
{
    public GameObject PlayerPrefab, SpawnPoint1, SpawnPoint2;

    void Start()
    {
        // On se connect directement au cloud de PUN, si vous avez sélectionné auto join dans la configuration
        // On devrait donc se connecter directement au Lobby
        PhotonNetwork.ConnectUsingSettings("tutopun");
    }

    // Après le Start() on rejoint le Lobby, c'est à dire qu'on est connecté au cloud de PUN
    // Maintenant il faut rejoindre une Room
    void OnJoinedLobby()
    {
        // On va créer une room pour 2 personnes maximun
        RoomOptions MyRoomOptions = new RoomOptions();
        MyRoomOptions.MaxPlayers = 2;

        // Ici on set de manière aléatoire le nom de l'utilisateur pour aller plus vite
        PhotonNetwork.playerName = "Player" + Random.Range(1, 500);

        // Enfin on rejoint la room demandé, du nom de "The Fight" mais on pourrait mettre un nom différent
        // afin de créer une autre room
        PhotonNetwork.JoinOrCreateRoom("The Fight", MyRoomOptions, TypedLobby.Default);
    }

    // Quand on a effectivement rejoint la room
    void OnJoinedRoom()
    {
        GameObject MyPlayer;
        // On instancie le joueur à tout le réseau
        // Nom du prefab à instancier / Position / Rotation / Groupe
        // Le groupe permet de différencier des joueurs, ou d'autre éléments, que vous vouliez différencier rapidement
        // les joueurs des adversaires OU différencier les équipes des joueurs
        if (PhotonNetwork.player.ID == 1)
        {
            MyPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPoint1.transform.position, Quaternion.identity, 0);
            MyPlayer.GetComponent<PlayerMoving>().hb = GameObject.Find("/HUD/HealthBar_P1").GetComponent<HealthBar>();
            MyPlayer.GetComponent<PlayerMoving>().zb = GameObject.Find("/HUD/ZemeBar_P1").GetComponent<ZemeBar>();
            //MyPlayer.GetComponent<PlayerMoving>().Enemy = PhotonView.Find(2).gameObject.GetComponent<PlayerMoving>();
            //Debug.Log(MyPlayer.GetComponent<PlayerMoving>().Enemy);
        }
        else
        {
            MyPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPoint2.transform.position, Quaternion.identity, 0);
            MyPlayer.GetComponent<PlayerMoving>().hb = GameObject.Find("/HUD/HealthBar_P2").GetComponent<HealthBar>();
            MyPlayer.GetComponent<PlayerMoving>().zb = GameObject.Find("/HUD/ZemeBar_P2").GetComponent<ZemeBar>();

        }
        // Le contrôle et la caméra sont désactivé afin d'être activé UNIQUEMENT pour le joueur en local
        // Et surtout pas contrôlé par les autres joueurs sur le réseau
        MyPlayer.GetComponent<PlayerMoving>().enabled = true;
        //MyPlayer.GetComponentInChildren<Camera>().enabled = true;
        //MyPlayer.GetComponentInChildren<Camera>().GetComponent<AudioListener>().enabled = true;
    }
}