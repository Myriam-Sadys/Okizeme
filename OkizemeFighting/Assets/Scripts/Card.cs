using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class Card : MonoBehaviour {
    public RawImage Image;
    public string Name;
    public string Description;
    public string Capacity;
    public string Power;
    public string Type;
    public string Id;
    public int ZP;
    public int Life_points;
    public int Stamina_points;
    public int Combo_bar_size;
    public int Assist_call_cost;
    public List<FightingMoves> Fighting_moves;

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void setData(CardData.Card card)
    {
        Image.texture = card.image;
        Name = card.name;
        Description = card.description;
        Capacity = card.capacity;
        Power = card.power;
        Type = card.type;
        Id = card.id;
        ZP = card.zp;
        Life_points = card.life_points;
        Stamina_points = card.stamina_points;
        Combo_bar_size = card.combo_bar_size;
        Assist_call_cost = card.assist_call_cost;
        Fighting_moves = new List<FightingMoves>();
        Fighting_moves.AddRange(card.fighting_moves);
    }
}
