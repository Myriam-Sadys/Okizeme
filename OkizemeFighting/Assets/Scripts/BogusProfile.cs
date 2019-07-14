using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using SimpleJSON;

public class BogusProfile : MonoBehaviour {
    public Transform GalleryScrollView;
    public GameObject cardPrefab;
    public CardData data;
    public RawImage displayedCard;
    public GameObject windowCard;
    public Canvas LoadingScreen;
    public Canvas LogoAnimation;

    private bool GalleryFilled;
    private string _token;
    public string Token
    {
        get { return _token; }
        set { _token = value; }
    }


    // Use this for initialization
    void Start() {
        LogoAnimation.gameObject.SetActive(true);
        StartCoroutine(WaitEndAnimation());
        GalleryFilled = false;
    }

    void onCardClicked(Card clickedCard)
    {
        windowCard.SetActive(true);
        //EditDisplayedCard(clickedCard);
    }

    // Update is called once per frame
    void Update () {
        //data.TotalCards;
        if (data.GalleryCards.Count > 3 && !GalleryFilled)
        {
            FillInGallery();
            GalleryFilled = true;
            (LoadingScreen.GetComponent(typeof(LoadingBar)) as LoadingBar).Hide();
        }
        //if (data.GalleryCards.Count != GalleryScrollView.transform.childCount)
        //{
        //    List <GameObject> listTMP = new List<GameObject>(GalleryScrollView.GetComponents<GameObject>());
        //    foreach (GameObject g in listTMP)
        //        Destroy(g);
        //    FillInGallery();
        //}
    }

    void FillInGallery()
    {
        foreach (CardData.Card c in data.GalleryCards)
        {
            Text[] TextComponent = cardPrefab.GetComponentsInChildren<Text>();
            TextComponent[0].text = c.name;
            TextComponent[1].text = c.life_points.ToString();
            GameObject g = GameObject.Instantiate(cardPrefab, GalleryScrollView);
            Card ca = g.GetComponent<Card>();
            ca.setData(c);
            Button button = g.GetComponent<Button>();
            button.onClick.AddListener(() => this.onCardClicked(ca));
        }
        foreach (CardData.Card c in data.HandCards)
        {
            Text[] TextComponent = cardPrefab.GetComponentsInChildren<Text>();
            TextComponent[0].text = c.name;
            TextComponent[1].text = c.life_points.ToString();
            GameObject g = GameObject.Instantiate(cardPrefab, GalleryScrollView);
            Card ca = g.GetComponent<Card>();
            ca.setData(c);
            Button button = g.GetComponent<Button>();
            button.onClick.AddListener(() => this.onCardClicked(ca));
        }
        EditDisplayedCard(data.GalleryCards[0]);
    }

    void EditDisplayedCard(CardData.Card card)
    {
        Text[] TextComponent = windowCard.GetComponentsInChildren<Text>();
        Button[] ButtonComponent = windowCard.GetComponentsInChildren<Button>();
        ButtonComponent[1].gameObject.GetComponent<SpecialAttackButton>().Hability = card.fighting_moves.Find(x => x.Type == "special_attack_1").Hability;
        ButtonComponent[2].gameObject.GetComponent<SpecialAttackButton>().Hability = card.fighting_moves.Find(x => x.Type == "special_attack_2").Hability;
        ButtonComponent[3].gameObject.GetComponent<SpecialAttackButton>().Hability = card.fighting_moves.Find(x => x.Type == "special_attack_3").Hability;
        TextComponent[0].text = card.fighting_moves.Find(x => x.Type == "jump_height").TypeValue.ToString();
        TextComponent[1].text = card.fighting_moves.Find(x => x.Type == "move_speed").TypeValue.ToString();
        TextComponent[2].text = card.fighting_moves.Find(x => x.Type == "standard_aerial_attack").Hability.Damages.ToString() + "/" +
                                card.fighting_moves.Find(x => x.Type == "powerful_aerial_attack").Hability.Damages.ToString();
        TextComponent[3].text = card.fighting_moves.Find(x => x.Type == "standard_attack").Hability.Damages.ToString() + "/" +
                                card.fighting_moves.Find(x => x.Type == "powerful_attack").Hability.Damages.ToString();
        TextComponent[4].text = card.combo_bar_size.ToString();
        TextComponent[5].text = card.stamina_points.ToString();
        TextComponent[6].text = card.name;
        TextComponent[7].text = card.description;
        TextComponent[9].text = card.capacity;
        TextComponent[11].text = card.type;
        TextComponent[12].text = card.life_points.ToString();
        displayedCard.texture = card.image;
    }

    IEnumerator WaitEndAnimation()
    {
        yield return new WaitForSeconds(10f);
        LogoAnimation.gameObject.SetActive(false);
    }
}
