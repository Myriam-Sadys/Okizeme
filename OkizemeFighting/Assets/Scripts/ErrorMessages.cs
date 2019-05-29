using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessages : MonoBehaviour {

    public Text Message;
    private Color AlphaColor;
    private float TimeToFade = 1.0f;
    private bool canFade;
    public bool CanFade
    {
        get { return canFade; }
        set
        {
            if (value != canFade)
                canFade = value;
        }
    }

    // Use this for initialization
    void Start () {
        //CanFade = false;
        //AlphaColor = this.GetComponent<MeshRenderer>().material.color;
        //AlphaColor.a = 0;
    }
	
	// Update is called once per frame
	void Update () {
        //if (canFade)
        //{
        //    if (this.AlphaColor.a <= 0)
        //        MakeVisible();
        //    this.AlphaColor = new Color(0, 0, 0, TimeToFade * Time.deltaTime);
        //    Message.color = new Color(0, 0, 0, TimeToFade * Time.deltaTime);
        //}
    }

    void MakeVisible()
    {
        //this.AlphaColor = new Color(0, 0, 0, 255);
        //Message.color = new Color(0, 0, 0, 255);
    }
}
