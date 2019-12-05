using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NbOfPlayers: Photon.PunBehaviour
{
    public override void OnLobbyStatisticsUpdate()
    {
        string countPlayersOnline;
        countPlayersOnline = PhotonNetwork.countOfPlayers.ToString() + " Players Online";
        Debug.Log(countPlayersOnline);
    }
}
