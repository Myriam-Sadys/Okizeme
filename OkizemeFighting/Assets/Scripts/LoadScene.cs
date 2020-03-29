using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string Name;

    public void OnClick()
    {
        SceneManager.LoadScene(Name, LoadSceneMode.Single);
    }
}
