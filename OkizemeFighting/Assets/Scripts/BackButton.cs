using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    [SerializeField]
    string SceneToLoad = "Menu";

    public void LoadScene()
    {
        Application.LoadLevel(SceneToLoad);
        //Application.Quit();
    }
}
