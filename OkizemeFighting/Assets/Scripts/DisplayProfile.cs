using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class DisplayProfile : MonoBehaviour {
    public Button modifyButton;

    public GameObject myGO;

    public InputField emailInputField;

    public InputField passwordInputField;

    public Button emailModify;

    public Button passwordModify;

    public Text pseudoText;
    public string password;

    public string token;

    public Image Warning;




    // Use this for initialization
    void Start () {
        token = GameInstanceManager.m_gim.m_token;
        emailInputField.text = GameInstanceManager.m_gim.m_email;
        emailInputField.interactable = false;
        passwordInputField.text = GameInstanceManager.m_gim.m_password;
        passwordInputField.interactable = false;
        pseudoText.text = GameInstanceManager.m_gim.m_userName;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
/*
    IEnumerator WaitForRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();
        Debug.Log("WWW : " + www.downloadHandler.text); 
    }
*/
    // UpdateAccount est utilisé pour modifier son compte une fois le bouton "modify" cliqué.  
    public void UpdateAccount()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        WWWForm form = new WWWForm();
        UnityWebRequest www;
        string APIurl = "";
        APIurl = "https://api.okizeme.dahobul.com/";
 
        form.AddField("token", token);
        if (email != "")
            form.AddField("email_address", email);
        if (password != "")
            form.AddField("password", password);
        byte[] formByte = form.data;
        Debug.Log(formByte[0]);
        Debug.Log(formByte[1]);
        Debug.Log(formByte[2]);
        Debug.Log(formByte[3]);
        Debug.Log(formByte[4]);
        Debug.Log(formByte[5]);

        www = UnityWebRequest.Post(APIurl + "accounts", form);
        www.method = "PATCH";
        StartCoroutine(WaitForRequest(www));
        Debug.Log("Update Account : " + Time.time);
    }

       IEnumerator WaitForRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();


        if (string.IsNullOrEmpty(www.downloadHandler.text))
            ErrorMessage("Connection Impossible");
        Debug.Log("WWW : " + www.downloadHandler.text);
    }

   void ErrorMessage(string message)
    {
        Debug.Log("Error: " + message);
        Animator WarningAnim = Warning.GetComponent<Animator>();
        Text text = Warning.GetComponentInChildren<Text>();
        text.text = message;
        WarningAnim.SetTrigger("Display");
    }

    public void onEmailModify() {
        if  (emailInputField.interactable)
            emailInputField.interactable = false;
        else
            emailInputField.interactable = true;
    }

    public void onPasswordModify() {
        if  (passwordInputField.interactable)
            passwordInputField.interactable = false;
        else
            passwordInputField.interactable = true;
    }
}
