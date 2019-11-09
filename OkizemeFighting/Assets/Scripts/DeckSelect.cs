using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelect : MonoBehaviour
{
    public Transform DeckScrollView;
    public GameObject DeckPrefab;
    public GameObject DeckSelectGameObject;

    private PlayerData playerInformations;
    public PlayerData PlayerInformations
    {
        get { return playerInformations; }
        set
        {
            if (value != playerInformations)
                playerInformations = value;
        }
    }

    private List<Deck> decks;
    public List<Deck> Decks
    {
        get { return decks; }
        set
        {
            if (value != decks)
                decks = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Decks = new List<Deck>();
        PlayerInformations = GameObject.Find("PlayerInformations").GetComponent<PlayerData>();
        foreach (PlayerData.DeckData d in PlayerInformations.Decks)
        {
            SetDeck(d);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnDeckClick(List<CardData.Card> DeckCards)
    {
        //HandCards.Clear();
        //HandCards.AddRange(DeckCards);
        DeckSelectGameObject.SetActive(false);
        //this.gameObject.SetActive(false);
        //UpdateHand = true;
    }

    public void SetDeck(PlayerData.DeckData DeckData)
    {
        GameObject g = GameObject.Instantiate(DeckPrefab, DeckScrollView);
        Deck deck = g.GetComponent<Deck>();
        deck.InitiateDeck(DeckData.name, DeckData.id);
        g.GetComponent<Button>().onClick.AddListener(() => this.OnDeckClick(deck.Cards));
        deck.Cards.AddRange(DeckData.Cards);
        Decks.Add(deck);
    }
}
