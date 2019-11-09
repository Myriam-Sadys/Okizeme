using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforePlayPlayersInformations : MonoBehaviour
{
    public GameObject GeneralCardsMaker;
    public GameObject PlayerInformations;

    public void OnClick()
    {
        PlayerInformations.GetComponent<PlayerData>().UserName = GeneralCardsMaker.GetComponent<CardData>().ProfileName.text;
        PlayerInformations.GetComponent<PlayerData>().Token = GeneralCardsMaker.GetComponent<CardData>().Token;
        PlayerInformations.GetComponent<PlayerData>().SetDecks(GeneralCardsMaker.GetComponent<CardData>().Decks);
    }
}
