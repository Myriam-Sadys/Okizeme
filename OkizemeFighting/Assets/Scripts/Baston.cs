using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SA
{
    public class Baston : MonoBehaviour
    {
        public void bastonscene()
        {
            //SceneManager.LoadScene(3, LoadSceneMode.Single);
            Fight.IsFight = false;
//            Scene currentScene = SceneManager.GetActiveScene();
        }
    }
}
