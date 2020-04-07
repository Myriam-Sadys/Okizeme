using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Connection : MonoBehaviour
{

    public Canvas Menu;
    public Canvas ConnectionWindow;
    public Canvas LoadingScreen;
    public Image Warning;
    public GameObject CreateAccountWindow;
    [SerializeField]
    private InputField Email;
    [SerializeField]
    private InputField Pass;

    private string APIurl;
    private int ConnectionTry;
    private bool CreateAccount;
    private bool Connect;
    private bool SetInfos;
    private string ConfirmPassword;
    private string UserName;
    private string UserAdress;
    private string Password;
    private string Token;

    void Start()
    {
        //APIurl = "https://api.okizeme.dahobul.com/";
        //ConnectionTry = 0;
        //CreateAccount = false;
        //UserName = GameInstanceManager.m_gim.m_userName;
        //UserAdress = GameInstanceManager.m_gim.m_email;
        //ConfirmPassword = "";
        //if (GameInstanceManager.m_gim.m_password != "")
        //    Password = GameInstanceManager.m_gim.m_password;
        //Token = GameInstanceManager.m_gim.m_token;
        //Debug.Log("Token: " + Token);
        //if (Token != "" && Password != "")
        //{
        //    Connect = true;
        //    // Login();
        //}
        //else
        //{
        //    Connect = false;
        //    if (UserAdress != "")
        //        SetInfos = true;
        //    Menu.gameObject.SetActive(false);
        //}
    }

    public void Awake()
    {
        APIurl = "https://api.okizeme.dahobul.com/";
        ConnectionTry = 0;
        CreateAccount = false;
        UserName = GameInstanceManager.m_gim.m_userName;
        UserAdress = GameInstanceManager.m_gim.m_email;
        ConfirmPassword = "";
        if (GameInstanceManager.m_gim.m_password != "")
            Password = GameInstanceManager.m_gim.m_password;
        Token = GameInstanceManager.m_gim.m_token;
        Debug.Log("Token: " + Token);
        if (Token != "" && Password != "")
        {
            Connect = true;
            // Login();
        }
        else
        {
            if (UserAdress != "")
                SetInfos = true;
            Connect = false;
            Menu.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Connect)
        {
            Debug.Log("Connect condition : " + Time.time);
            Menu.gameObject.SetActive(true);
            Menu.gameObject.GetComponent<BogusProfile>().Token = Token;
            Menu.gameObject.GetComponentInChildren<CardData>().Token = Token;
            ConnectionWindow.gameObject.SetActive(false);
        }

        if (SetInfos)
        {
            if (UserAdress != "")
                Email.text = UserAdress;
            if (Password != "")
                Pass.text = Password;
            SetInfos = false;
        }
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;
        try
        {
            return Regex.IsMatch(email, @"^.+[0-9a-z](\@).+[0-9a-z](\.).+[a-z]$", RegexOptions.IgnoreCase);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void SetConnection()
    {
        List<InputField> InputFields = new List<InputField>(CreateAccountWindow.GetComponentsInChildren<InputField>());
        Connect = false;

        UserAdress = InputFields.Find(x => x.name == "UserInputField").text;
        Password = InputFields.Find(x => x.name == "PasswordInputField").text;

        if (IsValidEmail(UserAdress))
        {
            if (InputFields.Count > 2)
            {
                UserName = InputFields.Find(x => x.name == "UserNameInputField").text;
                ConfirmPassword = InputFields.Find(x => x.name == "ConfirmPasswordInputField").text;
                if (Password != ConfirmPassword)
                    ErrorMessage("Password Missmatched");
                else
                {
                    NewAccount();
                    Login();
                }
            }
            else
                Login();
            Debug.Log("Exit conditions : " + Time.time);
        }
        else
        {
            ErrorMessage("Adress Invalid");
        }
    }

    void ErrorMessage(string message)
    {
        Debug.Log("Error: " + message);
        Animator WarningAnim = Warning.GetComponent<Animator>();
        Text text = Warning.GetComponentInChildren<Text>();
        text.text = message;
        WarningAnim.SetTrigger("Display");
    }

    IEnumerator WaitForRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        (LoadingScreen.GetComponent(typeof(LoadingBar)) as LoadingBar).Hide();
        if (string.IsNullOrEmpty(www.downloadHandler.text))
            ErrorMessage("Connection Impossible");
        else
            ConnectAccount(www.downloadHandler.text);
        Debug.Log("WWW : " + www.downloadHandler.text);
    }

    void ConnectAccount(string text)
    {
        var data = JSON.Parse(text);
        Animator WarningAnim = Warning.GetComponent<Animator>();

        switch (data["message"].ToString())
        {
            case "\"logged_in\"":
                string toEdit = data["token"].ToString();
                Token = toEdit.Substring(1, toEdit.Length - 2);
                Connect = true;
                SetUserData();
                break;
            case "\"invalid_username\"":
                ErrorMessage("Username Incorrect");
                break;
            case "\"invalid_password\"":
                ErrorMessage("Password Incorrect");
                break;
            case "\"invalid_email_address\"":
                ErrorMessage("Invalid Adress");
                break;
            case "\"username_already_used\"":
                ErrorMessage("Username is already used");
                break;
            case "\"email_address_already_used\"":
                ErrorMessage("Email is already used");
                break;
            case "\"invalid_credentials\"":
                ErrorMessage("Unknown account");
                ConnectionTry++;
                Debug.Log("Connection try: " + ConnectionTry);
                if (ConnectionTry == 4)
                {
                    CreateAccountWindow.GetComponent<Animator>().SetTrigger("Display");
                    CreateAccount = !CreateAccount;
                    ConnectionTry = 0;
                }
                break;
        }
    }

    void NewAccount()
    {
        WWWForm form = new WWWForm();
        UnityWebRequest www;

        form.AddField("username", UserName);
        form.AddField("email_address", UserAdress);
        form.AddField("password", Password);
        www = UnityWebRequest.Post(APIurl + "accounts", form);
        (LoadingScreen.GetComponent(typeof(LoadingBar)) as LoadingBar).Display();
        StartCoroutine(WaitForRequest(www));
        Debug.Log("New account : " + Time.time);
    }

    void Login()
    {
        WWWForm form = new WWWForm();
        UnityWebRequest www;

        form.AddField("email_address", UserAdress);
        form.AddField("password", Password);
        www = UnityWebRequest.Post(APIurl + "login", form);
        (LoadingScreen.GetComponent(typeof(LoadingBar)) as LoadingBar).Display();
        StartCoroutine(WaitForRequest(www));
        Debug.Log("Login : " + Time.time);
    }

    void SetUserData()
    {
        GameInstanceManager.m_gim.m_userName = UserName;
        GameInstanceManager.m_gim.m_email = UserAdress;
        GameInstanceManager.m_gim.m_password = Password;
        GameInstanceManager.m_gim.m_token = Token;
        PlayerPrefs.SetString("User", UserName);
        PlayerPrefs.SetString("Email", UserAdress);
        PlayerPrefs.SetString("Password", Password);
        PlayerPrefs.SetString("Token", Token);
        Menu.gameObject.SetActive(true);
        Menu.gameObject.GetComponent<BogusProfile>().Token = Token;
        Menu.gameObject.GetComponentInChildren<CardData>().Token = Token;
        ConnectionWindow.gameObject.SetActive(false);
    }
}
