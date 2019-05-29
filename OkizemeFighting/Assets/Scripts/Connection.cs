using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Connection : MonoBehaviour {

    public Canvas Menu;
    public Canvas ConnectionWindow;
    public Canvas LoadingScreen;
    public Image Warning;
    public GameObject CreateAccountWindow;

    private string APIurl;
    private int ConnectionTry;
    private bool CreateAccount;
    private bool Connect;
    private string ConfirmPassword;
    private string UserName;
    private string UserAdress;
    private string Password;
    private string Token;

    void Start () {
        APIurl = "http://api.okizeme.dahobul.com/";
        ConnectionTry = 0;
        CreateAccount = false;
        UserName = "";
        ConfirmPassword = "";
        Token = "";
    }
	
	void Update () {
        if (Connect)
        {
            Debug.Log("Connect condition : " + Time.time);
            Menu.gameObject.SetActive(true);
            Menu.gameObject.GetComponent<BogusProfile>().Token = Token;
            Menu.gameObject.GetComponentInChildren<CardData>().Token = Token;
            ConnectionWindow.gameObject.SetActive(false);
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
            if (CreateAccount)
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

}
