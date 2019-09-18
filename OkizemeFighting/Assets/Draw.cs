using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour {
    public GameObject EquipmentCard;
    GameObject cardclone;

    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            Debug.Log("Pioche");
            
        }
    }
}
