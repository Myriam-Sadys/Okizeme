using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstanceManager : MonoBehaviour
{
    public static GameInstanceManager m_gim;


    public string m_userName { get; set; }
    public string m_password { get; set; }
    public string m_email { get; set; }
    public string m_token { get; set; }

    private void Awake()
    {
        if (m_gim == null)
        {
            DontDestroyOnLoad(gameObject);
            m_gim = this;
        }
        else if (m_gim != this)
            Destroy(gameObject);

        m_userName = PlayerPrefs.GetString("User", "");
        m_email = PlayerPrefs.GetString("Email", "");
        m_password = PlayerPrefs.GetString("Password", "");
        m_token = PlayerPrefs.GetString("Token", "");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnsetToken()
    {
        m_gim.m_token = "";
        PlayerPrefs.SetString("Token", "");
    }
}
