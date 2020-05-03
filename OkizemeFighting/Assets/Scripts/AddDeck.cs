using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using SimpleJSON;

public class AddDeck : MonoBehaviour
{
    public Button addButton;
    
    public Button Deck1;
    
    public Button Deck2;
    
    public Button Deck3;
    
    public Button Deck4;
    
    public Button Deck5;
    
    public Button Deck6;

    public Button deleteButton;

    public Button editButton;

    public string idDeck1;

    public string idDeck2;
    
    public string idDeck3;
    
    public string idDeck4;
    
    public string idDeck5;
    
    public string idDeck6;
    
    public int currentDeck;
    public Image Warning;

    public InputField deckName;

    public int currentDeckNb;

    public string nameCurrentDeck;
    public string idDeck;

    public string token;
    public GameObject DeckSelectGameObject;

    public string id1;

    public string id2;

    public string id3;

    public string id4;

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

    public List<string> cards;

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

    public Transform DeckScrollView;
    public GameObject DeckPrefab;
    // Start is called before the first frame update
    void Start()
    {
        token = GameInstanceManager.m_gim.m_token;
        deckName.text = "";
        idDeck1 = "";
        idDeck2 = "";
        idDeck3 = "";
        idDeck4 = "";
        idDeck5 = "";
        idDeck6 = "";
        nameCurrentDeck = "";
        currentDeck = 0;
    }

   void Update()
    {
    }
    
    public void displayDecks()
    {
        idDeck1 = "";
        idDeck2 = "";
        idDeck3 = "";
        idDeck4 = "";
        idDeck5 = "";
        idDeck6 = "";
        nameCurrentDeck = "";
        Deck1.gameObject.SetActive(false);
        Deck2.gameObject.SetActive(false);
        Deck3.gameObject.SetActive(false);
        Deck4.gameObject.SetActive(false);
        Deck5.gameObject.SetActive(false);
        Deck6.gameObject.SetActive(false);
        deckName.GetComponent<InputField>().text = "";
        token = GameInstanceManager.m_gim.m_token;
        WWWForm form = new WWWForm();
        UnityWebRequest www;
        string APIurl = "";
        APIurl = "https://api.okizeme.dahobul.com/";
        www = UnityWebRequest.Post(APIurl + "accounts/" + token + "/decks", form);
        www.method = "GET";
        StartCoroutine(WaitForRequest2(www));
}

    public void updateDeck()
    {
        string APIurl = "";
        APIurl = "https://api.okizeme.dahobul.com/";
        string idDeckUpdate = "";
        if (currentDeck == 1)
            idDeckUpdate = idDeck1;
        if (currentDeck == 2)
            idDeckUpdate = idDeck2;
        if (currentDeck == 3)
            idDeckUpdate = idDeck3;
        if (currentDeck == 4)
            idDeckUpdate = idDeck4;
        if (currentDeck == 5)
            idDeckUpdate = idDeck5;
        if (currentDeck == 6)
            idDeckUpdate = idDeck6;
        
        token = GameInstanceManager.m_gim.m_token;
        string requestData = "";
        string listCard = string.Join("\", \"", cards.ToArray());
        requestData = "?deck_data={\"name\": \"" + nameCurrentDeck + "\", \"cards\": [\"" + listCard + "\"]}";
        Debug.Log(requestData);
        //byte[] myData = System.Text.Encoding.UTF8.GetBytes(requestData);
        UnityWebRequest www;
        WWWForm form = new WWWForm();
        form.AddField("deck_data", requestData);
        byte[] formByte = form.data;
        string json = JsonUtility.ToJson(requestData);
        www = UnityWebRequest.Put(APIurl + "accounts/" + token + "/decks/" + idDeckUpdate + requestData, formByte);
        //www.method = "PUT";

        //www.SetRequestHeader("Content-Type", "application/json");
        //www.SetRequestHeader("Accept", "application/json");
        StartCoroutine(WaitForRequestUpdateDeck(www));
}
  IEnumerator WaitForRequestUpdateDeck(UnityWebRequest www)
    {
        yield return www.SendWebRequest();
        Debug.Log("WWW : " + www.downloadHandler.text);
    }

       IEnumerator WaitForRequest2(UnityWebRequest www)
    {
        yield return www.SendWebRequest();


        if (string.IsNullOrEmpty(www.downloadHandler.text))
            ErrorMessage("Récupération de decks impossible");
        adjustButton(www);
    }

    public void adjustButton(UnityWebRequest www) {
        var data = JSON.Parse(www.downloadHandler.text);
        var dataDeck = data["data"];
        currentDeckNb = dataDeck.Count;
        int i = 0;
        Debug.Log("WWW : " + www.downloadHandler.text);
        while (i < currentDeckNb)
        {
            if (i == 0)
            {
                Deck1.GetComponentInChildren<Text>().text = dataDeck[i]["name"];
                idDeck1 = dataDeck[i]["_id"]["$oid"];
                Deck1.gameObject.SetActive(true);
            }
            if (i == 1)
            {
                Deck2.GetComponentInChildren<Text>().text = dataDeck[i]["name"];
                idDeck2 = dataDeck[i]["_id"]["$oid"];
                Deck2.gameObject.SetActive(true);
            }
            if (i == 2)
            {
                Deck3.GetComponentInChildren<Text>().text = dataDeck[i]["name"];
                idDeck3 = dataDeck[i]["_id"]["$oid"];
                Deck3.gameObject.SetActive(true);
            }
            if (i == 3)
            {
                Deck4.GetComponentInChildren<Text>().text = dataDeck[i]["name"];
                idDeck4 = dataDeck[i]["_id"]["$oid"];
                Deck4.gameObject.SetActive(true);
            }
            if (i == 4)
            {
                Deck5.GetComponentInChildren<Text>().text = dataDeck[i]["name"];
                idDeck5 = dataDeck[i]["_id"]["$oid"];
                Deck5.gameObject.SetActive(true);
            }
            if (i == 5)
            {
                Deck6.GetComponentInChildren<Text>().text = data[i]["name"];
                idDeck6 = dataDeck[i]["_id"]["$oid"];
                Deck6.gameObject.SetActive(true);
            }
            i++;
        }
    }

    public void OnDeckClick(List<CardData.Card> DeckCards)
    {
        //HandCards.Clear();
        //HandCards.AddRange(DeckCards);
        DeckSelectGameObject.SetActive(false);
        //this.gameObject.SetActive(false);
        //UpdateHand = true;
    }

    // Update is called once per frame
 

    /*public void SetDeck(PlayerData.DeckData DeckData)
    {
        GameObject g = GameObject.Instantiate(DeckPrefab, DeckScrollView);
        Deck deck = g.GetComponent<Deck>();
        deck.InitiateDeck(DeckData.name, DeckData.id);
        g.GetComponent<Button>().onClick.AddListener(() => this.OnDeckClick(deck.Cards));
        deck.Cards.AddRange(DeckData.Cards);
        Decks.Add(deck);
    }*/

    public void newDeck()
    {
        string name = deckName.text;
        if (string.IsNullOrEmpty(name)) {
            ErrorMessage("Création de deck impossble");
            return;    
        }

        WWWForm form = new WWWForm();
        UnityWebRequest www;
        string APIurl = "";
        APIurl = "https://api.okizeme.dahobul.com/";
 
        form.AddField("deck_name", name);
        byte[] formByte = form.data;
        Debug.Log(formByte[0]);
        Debug.Log(formByte[1]);
        www = UnityWebRequest.Post(APIurl + "accounts/" + token + "/decks", form);
        www.method = "POST";
        StartCoroutine(WaitForRequest(www));
    }

    
       IEnumerator WaitForRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();


        if (string.IsNullOrEmpty(www.downloadHandler.text))
            ErrorMessage("Création de deck impossible");
        Debug.Log("WWW : " + www.downloadHandler.text);
        var data = JSON.Parse(www.downloadHandler.text);
        idDeck = data["data"];
        Debug.Log("Id deck : " + idDeck);
    }

   void ErrorMessage(string message)
    {
        Debug.Log("Error: " + message);
        Animator WarningAnim = Warning.GetComponent<Animator>();
        Text text = Warning.GetComponentInChildren<Text>();
        text.text = message;
        WarningAnim.SetTrigger("Display");
    }

    public void ClickDeck1()
    {
        currentDeck = 1;
        nameCurrentDeck = Deck1.GetComponentInChildren<Text>().text;
    }

   public void ClickDeck2()
    {
        currentDeck = 2;
        nameCurrentDeck = Deck2.GetComponentInChildren<Text>().text;
    }

    public void ClickDeck3()
    {
        currentDeck = 3;
        nameCurrentDeck = Deck3.GetComponentInChildren<Text>().text;
    }

    public void ClickDeck4()
    {
        currentDeck = 4;
        nameCurrentDeck = Deck4.GetComponentInChildren<Text>().text;
    }

    public void ClickDeck5()
    {
        currentDeck = 5;
        nameCurrentDeck = Deck5.GetComponentInChildren<Text>().text;
    }

    public void ClickDeck6()
    {
        currentDeck = 6;
        nameCurrentDeck = Deck6.GetComponentInChildren<Text>().text;
    }

    public void deleteDeck()
    {
        token = GameInstanceManager.m_gim.m_token;
        string id = "";
        if (currentDeck == 0)
            return;
        if (currentDeck == 1)
            id = idDeck1;
        if (currentDeck == 2)
            id = idDeck2;
        if (currentDeck == 3)
            id = idDeck3;
        if (currentDeck == 4)
            id = idDeck4;
        if (currentDeck == 5)
            id = idDeck5;
        if (currentDeck == 6)
            id = idDeck6;

        UnityWebRequest www;
        string APIurl = "";
        APIurl = "https://api.okizeme.dahobul.com/";
        www = UnityWebRequest.Delete(APIurl + "accounts/" + token + "/decks/" + id);
        StartCoroutine(WaitForRequestDelete(www));
    }

    IEnumerator WaitForRequestDelete(UnityWebRequest www)
    {
        yield return www.SendWebRequest();
        Debug.Log("WWW : " + www.downloadHandler.text);
    }
    
    public void storeCardSelected()
    {
        cards = new List<string>();
        if (DeckScrollView.transform.childCount == 0)
            return;
        if (DeckScrollView.transform.childCount == 1)
            cards.Add(id1);
        else if (DeckScrollView.transform.childCount == 2)
        {
            cards.Add(id1);
            cards.Add(id2);
        }
        else if (DeckScrollView.transform.childCount == 3)
        {
            cards.Add(id1);
            cards.Add(id2);
            cards.Add(id3);
        }
        else if (DeckScrollView.transform.childCount == 4)
        {
            cards.Add(id1);
            cards.Add(id2);
            cards.Add(id3);
            cards.Add(id4);
        }
        updateDeck();
    }

}
