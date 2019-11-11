using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitApplication : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Quit()
    {
        PlayerPrefs.SetString("User", "");
        PlayerPrefs.SetString("Email", "");
        PlayerPrefs.SetString("Password", "");
        PlayerPrefs.SetString("Token", "");


        Application.Quit();
    }
}
