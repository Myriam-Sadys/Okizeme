using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindName : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text _textField = this.GetComponent<Text>();

        if (PhotonNetwork.playerName != "")
        {
            _textField.text = PhotonNetwork.playerName;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
