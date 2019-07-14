using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class ScrollViews : MonoBehaviour
{
    public Transform Gallery;
    public Transform Hand;
    public CardData data;
    public GameObject cardPrefab;
    private List<GameObject> HandCards;
    private List<GameObject> GalleryCards;

    void Start()
    {
        HandCards = new List<GameObject>();
        GalleryCards = new List<GameObject>();
        data.UpdateHand = true;
    }

    void InstantiateGalleryCards(CardData.Card card)
    {
        Text[] TextComponent = cardPrefab.GetComponentsInChildren<Text>();
        TextComponent[0].text = card.name;
        TextComponent[1].text = card.zp.ToString();
        GameObject g = GameObject.Instantiate(cardPrefab, Gallery);
        Card ca = g.GetComponent<Card>();
        ca.setData(card);
        Button button = g.GetComponent<Button>();
        button.onClick.AddListener(() => this.onCardClickedGallery(ca));
        GalleryCards.Add(g);
    }

    void InstantiateHandCards(CardData.Card card)
    {
        Text[] TextComponent = cardPrefab.GetComponentsInChildren<Text>();
        TextComponent[0].text = card.name;
        TextComponent[1].text = card.zp.ToString();
        GameObject g = GameObject.Instantiate(cardPrefab, Hand);
        Card ca = g.GetComponent<Card>();
        ca.setData(card);
        Button button = g.GetComponent<Button>();
        button.onClick.AddListener(() => this.onCardClickedHand(ca));
        HandCards.Add(g);
    }

    // Update is called once per frame
    void Update()
    {
        if (data.UpdateHand)
        {
            GalleryCards = emptyScrollView(GalleryCards);
            HandCards = emptyScrollView(HandCards);
            foreach (CardData.Card c in data.GalleryCards)
                InstantiateGalleryCards(c);
            foreach (CardData.Card c in data.HandCards)
                InstantiateHandCards(c);
            data.UpdateHand = false;
        }
    }

    List<GameObject> emptyScrollView(List<GameObject> scrollView)
    {
        foreach (GameObject g in scrollView)
            Destroy(g);
        scrollView.Clear();
        return scrollView;
    }

    void onCardClickedGallery(Card clickedCard)
    {
        var changeToHand = data.GalleryCards.Single(x => x.id == clickedCard.Id);
        data.HandCards.Add(changeToHand);
        InstantiateHandCards(changeToHand);
        GalleryCards.RemoveAll(x => x == null);
        data.GalleryCards.Remove(changeToHand);
        Destroy(GalleryCards.Single(x => x.GetComponent<Card>().Id == clickedCard.Id));
    }

    void onCardClickedHand(Card clickedCard)
    {
        var changeToGallery = data.HandCards.Single(x => x.id == clickedCard.Id);
        data.GalleryCards.Add(changeToGallery);
        InstantiateGalleryCards(changeToGallery);
        HandCards.RemoveAll(x => x == null);
        data.HandCards.Remove(changeToGallery);
        Destroy(HandCards.Single(x => x.GetComponent<Card>().Id == clickedCard.Id));
    }
}
