using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollScript : MonoBehaviour {

    float scrollspeed = -50f;
    Vector2 startPos;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
        startPos += new Vector2(-600, 300);
    }

    // Update is called once per frame
    void Update () {
        float newPos = Mathf.Repeat(Time.time * scrollspeed, 1200);
        transform.position = startPos + Vector2.right * newPos;
    }
}
