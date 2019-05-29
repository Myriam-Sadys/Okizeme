using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayProfile : MonoBehaviour {
    public Texture2D pic;
    public string ProfileName;
    public string profile;

    public Text profileName;
    public Text profileBio;
    public RawImage displayedPic;

    // Use this for initialization
    void Start () {
        displayedPic.texture = pic;
        profileBio.text = profile;
        profileName.text = ProfileName;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
