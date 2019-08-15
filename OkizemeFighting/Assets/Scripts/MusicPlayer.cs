using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {
    public AudioSource audioSourceLoop;
    public AudioSource audioSourceBegin;
    //private AudioSource audioSource
    private float musicVolume;
    private bool FadeIn;

    // Use this for initialization
    void Start() {
        FadeIn = false;
        //audioSource = audioSourceBegin;
        audioSourceBegin.volume = 0f;
        audioSourceBegin.Play();
        //DontDestroyOnLoad(transform.gameObject);
    }

    public void PlayMusic()
    {
        if (audioSourceLoop.isPlaying) return;
        audioSourceLoop.Play();
    }

    // Update is called once per frame
    void Update () {
        if (audioSourceBegin.volume < 1)
            audioSourceBegin.volume = audioSourceBegin.volume + (Time.deltaTime / (4));
        if (!audioSourceBegin.isPlaying && !FadeIn)
        {
            FadeIn = true;
            //audioSource.volume = musicVolume;
            //audioSource = null;
            //audioSource = audioSourceLoop;
            musicVolume = audioSourceBegin.volume;
            audioSourceLoop.Play();
        }
        //if (FadeIn)
        audioSourceLoop.volume = musicVolume;
    }

    public void setVolume(float vol)
    {
        musicVolume = vol;
        audioSourceLoop.volume = vol;
    }

    public void StopMusic()
    {
        audioSourceLoop.Stop();
    }
}

