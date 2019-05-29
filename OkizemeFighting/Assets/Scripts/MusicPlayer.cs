using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {
    public AudioSource audioSource;
    private float musicVolume;

	// Use this for initialization
	void Start () {
        //audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update () {
        audioSource.volume = musicVolume;
	}

    public void setVolume(float vol)
    {
        musicVolume = vol;
    }
}
