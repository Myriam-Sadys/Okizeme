using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{ 
    public void LoadScene()
    {
        Application.Quit();
    }
}
