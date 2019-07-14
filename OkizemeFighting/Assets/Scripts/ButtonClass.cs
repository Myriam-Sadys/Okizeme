using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClass : MonoBehaviour {
    public AudioClip sound;
    public AudioSource source;
    public Image border1;
    public Image border2;
    public Image border3;
    public Image border4;
    private bool changeBorder;
    private Vector3[] GeneratedPlace;
    private Vector3[] GeneratedRotation;

    // Use this for initialization
    void Start () {
        changeBorder = false;
        GeneratedPlace = new[] { new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f) };
        GeneratedRotation = new[] { new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f) };
    }

    public void PlaySound()
    {
        source.PlayOneShot(sound);
    }
	// Update is called once per frame
	void Update () {
		
	}

    public void StartMoving()
    {
        InvokeRepeating("BorderMove", 0.0f, 0.5f);
    }
    public void EndMoving()
    {
        if (changeBorder)
        {
            border1.transform.position += GeneratedPlace[0];
            border2.transform.position += GeneratedPlace[1];
            border2.transform.Rotate(GeneratedRotation[0]);
            border3.transform.position += GeneratedPlace[2];
            border4.transform.Rotate(GeneratedRotation[1]);
            changeBorder = false;
        }
        CancelInvoke("BorderMove");
    }

    void BorderMove()
    {
        if (changeBorder)
        {
            border1.transform.position += GeneratedPlace[0];
            border2.transform.position += GeneratedPlace[1];
            border2.transform.Rotate(GeneratedRotation[0]);
            border3.transform.position += GeneratedPlace[2];
            border4.transform.Rotate(GeneratedRotation[1]);
            changeBorder = false;
        }
        else
        {
            GeneratedPlace[0] = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
            GeneratedPlace[1] = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
            GeneratedPlace[2] = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
            GeneratedRotation[0] = new Vector3(Random.Range(-3.0f, 3.0f), 0.0f, Random.Range(-3.0f, 3.0f));
            GeneratedRotation[1] = new Vector3(Random.Range(-3.0f, 3.0f), 0.0f, Random.Range(-3.0f, 3.0f));
            border1.transform.position -= GeneratedPlace[0];
            border2.transform.position -= GeneratedPlace[1];
            border2.transform.Rotate(-GeneratedRotation[0]);
            border3.transform.position -= GeneratedPlace[2];
            border4.transform.Rotate(-GeneratedRotation[1]);
            changeBorder = true;
        }
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
