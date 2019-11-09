using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPlayerInformation : MonoBehaviour
{
    public GameObject PlayerInformations;
    // Start is called before the first frame update
    void Start()
    {
        PlayerInformations = GameObject.Find("PlayerInformations");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
