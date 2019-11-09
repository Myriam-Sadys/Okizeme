using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public int playerHealth;       // Reference to the player's health.
    Animator anim;                          // Reference to the animator component.


    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth <= 0)
        {
            // ... tell the animator the game is over.
            anim.SetTrigger("GameOver");
        }

    }
}
