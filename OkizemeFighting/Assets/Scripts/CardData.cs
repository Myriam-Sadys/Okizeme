using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CardData : MonoBehaviour {
    [System.Serializable]
    public class Card
    {
        public int zp;
        public string id;
        public string name;
        public string description;
        public Texture2D image;
        public string capacity;
        public string power;
        public string type;
        public int life_points;
        public int stamina_points;
        public int combo_bar_size;
        public int assist_call_cost;
        public List<FightingMoves> fighting_moves;
    }
    public Transform DeckScrollView;
    public GameObject DeckPrefab;
    public GameObject DeckSelect;
    public Text ProfileName;
    public Text ProfileAdress;

    private string APIurl;
    private List<Card> handCards;
    public List<Card> HandCards
    {
        get { return handCards; }
        set
        {
            if (value != handCards)
                handCards = value;
        }
    }

    private List<Card> galleryCards;
    public List<Card> GalleryCards
    {
        get { return galleryCards; }
        set
        {
            if (value != galleryCards)
                galleryCards = value;
        }
    }

    private string _token;
    public string Token
    {
        get { return _token; }
        set { _token = value; }
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

    private bool updateHand;
    public bool UpdateHand
    {
        get { return updateHand; }
        set { updateHand = value; }
    }

    private int totalCards;
    public int TotalCards
    {
        get { return totalCards; }
        set { totalCards = value; }
    }

    void Start()
    {
        TotalCards = 0;
        UpdateHand = false;
        HandCards = new List<Card>();
        GalleryCards = new List<Card>();
        APIurl = "http://api.okizeme.dahobul.com/";
        Decks = new List<Deck>();
        StartCoroutine(GetCardsFromAPI());
        StartCoroutine(SetAcountInformation());
    }

    void Update()
    {
    }

    public IEnumerator GetCardsFromAPI()
    {
        UnityWebRequest www = UnityWebRequest.Get(APIurl + "accounts/" + Token + "/decks");
        UnityWebRequest wwwCards = UnityWebRequest.Get(APIurl + "cards_info");
        yield return www.SendWebRequest();
        if (!string.IsNullOrEmpty(www.downloadHandler.text))
        {
            var data = JSON.Parse(www.downloadHandler.text);
            var smalldata = data["data"];
            Debug.LogError("smalldata values : " + smalldata);

            for (int i = 0; i != smalldata.Count; i++)
            {
                string DeckId = smalldata[i]["_id"]["$oid"];
                if (DeckId != null)
                    StartCoroutine(GetDeck(DeckId, smalldata[i]["name"]));
            }
            AddNewDeckButton();
            //ids.Add(smalldata[i]["_id"]["$oid"]);
        }

        yield return wwwCards.SendWebRequest();
        if (!string.IsNullOrEmpty(wwwCards.downloadHandler.text))
        {
            var data = JSON.Parse(wwwCards.downloadHandler.text);
            var smalldata = data["data"];
            Debug.LogError("smalldata values : " + smalldata);

            for (int i = 0; i != smalldata.Count; i++)
            {
                string CardId = smalldata[i]["_id"]["$oid"];
                if (CardId != null)
                    StartCoroutine(CreateNewCard(UnityWebRequest.Get(APIurl + "cards_info/" + CardId), null, false));
            }
            //AddNewDeckButton();
            //ids.Add(smalldata[i]["_id"]["$oid"]);
        }
    }

    public IEnumerator SetAcountInformation()
    {
        WWWForm form = new WWWForm();
        UnityWebRequest www;

        form.AddField("token", Token);
        www = UnityWebRequest.Post(APIurl + "token_info", form);
        yield return www.SendWebRequest();
        var data = JSON.Parse(www.downloadHandler.text);
        ProfileName.text = data["data"]["username"];
        ProfileAdress.text = data["data"]["email_address"];
    }


    IEnumerator GetDeck(string DeckId, string DeckName)
    {
        //DeckPrefab.GetComponent<Deck>().Name = DeckName;
        GameObject g = GameObject.Instantiate(DeckPrefab, DeckScrollView);
        Deck deck = g.GetComponent<Deck>();
        deck.InitiateDeck(DeckName, DeckId);
        g.GetComponent<Button>().onClick.AddListener(() => this.OnDeckClick(deck.Cards));
        UnityWebRequest www = UnityWebRequest.Get(APIurl + "accounts/" + Token + "/decks/" + DeckId);
        yield return www.SendWebRequest();
        var data = JSON.Parse(www.downloadHandler.text);
        var cardsdata = data["data"]["cards"];
        TotalCards += cardsdata.Count;
        List<string> ids = new List<string>();
        for (int i = 0; i != cardsdata.Count; i++)
            ids.Add(cardsdata[i]["$oid"]);
        for (int i = 0; i != ids.Count; i++)
        {
            string tmp = APIurl + "cards_info/" + ids[i];
            UnityWebRequest wwwCardID = UnityWebRequest.Get(APIurl + "cards_info/" + ids[i]);
            StartCoroutine(CreateNewCard(wwwCardID, deck.Cards, true));
        }
        Decks.Add(deck);
    }
    
    void AddNewDeckButton()
    {
        GameObject g = GameObject.Instantiate(DeckPrefab, DeckScrollView);
        Deck deck = g.GetComponent<Deck>();
        deck.InitiateDeck("New Deck", "");
        g.GetComponent<Button>().onClick.AddListener(() => this.OnDeckClick(deck.Cards));
    }

    IEnumerator CreateNewCard(UnityWebRequest www, List<CardData.Card> DeckCards, bool inDeck)
    {
        yield return www.SendWebRequest();
        var cardData = JSON.Parse(www.downloadHandler.text);
        var IdCardValues = cardData["data"];
        Card c = new Card();

        // TODO : make a better random part
        c.id = IdCardValues["_id"]["$oid"] + Random.Range(0f,1000f).ToString();
        c.name = IdCardValues["name"];
        c.description = IdCardValues["description"];
        c.type = IdCardValues["type"];
        c.life_points = IdCardValues["life_points"];
        c.stamina_points = IdCardValues["stamina_points"];
        c.combo_bar_size = IdCardValues["combo_bar_size"];
        c.assist_call_cost = IdCardValues["assist_call_cost"];
        c.capacity = IdCardValues["capacity"];
        c.fighting_moves = CreateNewFightingMoves(IdCardValues["fighting_moves"].ToString());

        string artworkString = IdCardValues["artwork"]["$oid"];
        UnityWebRequest wwwImage = UnityWebRequest.Get(APIurl + "artworks/" + artworkString);
        yield return wwwImage.SendWebRequest();
        c.image = GetImage(wwwImage.downloadHandler.text);

        if (inDeck)
            DeckCards.Add(c);
        else
            GalleryCards.Add(c);
    }

    Texture2D GetImage(string www)
    {
        var imageData = JSON.Parse(www);
        string b64_string = imageData["data"];
        byte[] b64_bytes = System.Convert.FromBase64String(b64_string.Substring(b64_string.IndexOf("base64,") + 7));
        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(b64_bytes);
        return tex;
    }

    List<FightingMoves> CreateNewFightingMoves(string JsonLine)
    {
        List<FightingMoves> list = new List<FightingMoves>();
        var fightingMoveData = JSON.Parse(JsonLine);
        list.Add(new FightingMoves("move_speed", fightingMoveData["move_speed"]));
        list.Add(new FightingMoves("jump_height", fightingMoveData["jump_height"]));
        list.Add(CreateNewHability(fightingMoveData["standard_attack"].ToString(), "standard_attack"));
        list.Add(CreateNewHability(fightingMoveData["powerful_attack"].ToString(), "powerful_attack"));
        list.Add(CreateNewHability(fightingMoveData["standard_aerial_attack"].ToString(), "standard_aerial_attack"));
        list.Add(CreateNewHability(fightingMoveData["powerful_aerial_attack"].ToString(), "powerful_aerial_attack"));
        list.Add(CreateNewHability(fightingMoveData["special_attack_1"].ToString(), "special_attack_1"));
        list.Add(CreateNewHability(fightingMoveData["special_attack_2"].ToString(), "special_attack_2"));
        list.Add(CreateNewHability(fightingMoveData["special_attack_3"].ToString(), "special_attack_3"));
        return list;
    }

    FightingMoves CreateNewHability(string JsonLine, string type)
    {
        var fightingMoveData = JSON.Parse(JsonLine);
        FightingMoves move = new FightingMoves(type, fightingMoveData["name"], fightingMoveData["description"], fightingMoveData["combo_cost"], fightingMoveData["damages"]);
        return move;
    }

    IEnumerator WaitForRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        if (!string.IsNullOrEmpty(www.downloadHandler.text))
            Debug.Log("WWW Ok!: " + www.downloadHandler.text);
        else
            Debug.Log("WWW Error: " + www.error);
    }

    public void OnDeckClick(List<Card> DeckCards)
    {
        HandCards.Clear();
        HandCards.AddRange(DeckCards);
        DeckSelect.gameObject.SetActive(false);
        UpdateHand = true;
    }
    
}
