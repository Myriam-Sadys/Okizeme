using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public class DeckData
    {
        public string id;
        public string name;
        public List<CardData.Card> Cards;
    }
    // Start is called before the first frame update
    private string _token;
    public string Token
    {
        get { return _token; }
        set { _token = value; }
    }

    private List<DeckData> _decks;
    public List<DeckData> Decks
    {
        get { return _decks; }
        set
        {
            if (value != _decks)
                _decks = value;
        }
    }

    private string _username;
    public string UserName
    {
        get { return _username; }
        set { _username = value; }
    }


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Decks = new List<DeckData>();
        Token = "";
        UserName = "";
    }

    public void SetDecks(List<Deck> decksToAdd)
    {
        foreach(Deck d in decksToAdd)
        {
            DeckData n = new DeckData();
            n.id = d.ID;
            n.name = d.Name;
            n.Cards = new List<CardData.Card>();
            n.Cards.AddRange(d.Cards);
            Decks.Add(n);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
