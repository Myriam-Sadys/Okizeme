using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalGame : MonoBehaviour
{
   public Animator transitionAnim;

    public void Changescene()
    { 
          StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        transitionAnim.Play("end");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }
}
