using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class charLimit : MonoBehaviour
{
    public InputField mainInputField;
    public int lengthMax;

    void Start()
    {
        //Changes the character limit in the main input field.
        mainInputField.characterLimit = lengthMax;
    }
}
