using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccountDisconnect : MonoBehaviour
{
    [SerializeField]
    string SceneToLoad = "Menu";
    [SerializeField]
    private GameObject AccountConnection;
    [SerializeField]
    private GameObject LoadingScreen;
    [SerializeField]
    private GameObject Canvas;
    [SerializeField]
    private GameObject MenuCanvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Disconnect()
    {
        GameInstanceManager.m_gim.UnsetToken();
        Application.LoadLevel(SceneToLoad);
    }
}
