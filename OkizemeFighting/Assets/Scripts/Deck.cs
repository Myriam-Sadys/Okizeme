using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Text DisplayedName;
    public Animator Animator;

    private string id;
    public string ID
    {
        get { return id; }
        set
        {
            if (id != value)
                id = value;
        }
    }
    
    private string name;
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    private List<CardData.Card> cards;
    public List<CardData.Card> Cards
    {
        get { return cards; }
        set { cards = value; }
    }

    void Start()
    {
        //DisplayedName.text = "Deck de test";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitiateDeck(string _name, string _id)
    {
        ID = _id;
        Name = _name;
        DisplayedName.text = _name;
        Cards = new List<CardData.Card>();
    }

    public void AddCard(CardData.Card card)
    {
        Cards.Add(card);
    }

    public void PointerEnter()
    {
        Animator.SetBool("Activate", true);
    }

    public void PointerExit()
    {
        Animator.SetBool("Activate", false);
    }
}
