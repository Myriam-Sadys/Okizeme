using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Baston : MonoBehaviour
{
    public void bastonscene()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Single);
        Scene currentScene = SceneManager.GetActiveScene();
    }
}
