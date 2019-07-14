using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public int maxMessages = 25;
    public int port = 6324;
    public GameObject chatPanel, textObject;
    public Color playerMessage, info;
    [SerializeField]
    List<Message> messageList = new List<Message>();

    public GameObject chatContainer;
    public string clientName;
    public InputField chatBox;
    public string APIurl = "api.okizeme.dahobul.com/lobby_chat_censored_words";
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    private GameObject color;
    private GameObject[] messages;

    public void ConnectToServer()
    {
        if (socketReady)
            return;
        string host = "127.0.0.1";
        int p = port;
        Server test = GetComponent<Server>();
        p = test.port;
        Debug.Log("Port : " + p + "host : " + host);
        try
        {
            socket = new TcpClient(host, p);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
            Debug.Log("Socket ready true");
        }
        catch (Exception e)
        {
            Debug.Log("Socket error : " + e.Message);
        }
    }

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
            }
            if (chatBox.text != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SendMessageToChat(chatBox.text, Message.MessageType.playerMessage);
                    chatBox.text = "";
                }
            }
        }
    }

    private void OnIncomingData(string data)
    {
        if (data == "%NAME")
        {
            Send("&NAME |" + clientName);
            return;
        }

        GameObject go = Instantiate(textObject, chatContainer.transform) as GameObject;
        go.GetComponentInChildren<Text>().text = data;
    }

    private void Send(string data)
    {
        if (!socketReady)
            return;
        writer.WriteLine(data);
        writer.Flush();
    }

    public void OnSendButton()
    {
        string message = GameObject.Find("SendInput").GetComponent<InputField>().text;
        Send(message);
    }

    public void CloseSocket()
    {
        if (!socketReady)
            return;
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }

    public static string Repeat(string value, int count)
    {
        return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
    }

    private string VerifyBadWords(string text, string badWords)
    {
        BadWords BWords = new BadWords();
        BWords = JsonUtility.FromJson<BadWords>(badWords);
        string tmp = text;
        foreach (string item in BWords.data)
        {
            tmp = ReplaceBadWords.ReplaceCaseInsensitive(tmp, item, Repeat("*", item.Length), StringComparison.OrdinalIgnoreCase);
            Debug.Log(item);
        }
        return tmp;
    }

    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        //if (messageList.Count >= maxMessages)
        //{
        //    Destroy(messageList[0].textObject.gameObject);
        //    messageList.Remove(messageList[0]);
        //}
        Message newMessage = new Message();
        newMessage.text = text;
        //GameObject newText = Instantiate(textObject, chatPanel.transform);
        //newMessage.textObject = newText.GetComponent<Text>();
        //newMessage.textObject.text = newMessage.text;
        messageList.Add(newMessage);

        StartCoroutine(GetBadWords((myReturnValue) =>
        {
            Send(VerifyBadWords(text, myReturnValue));
        }));

    }

    IEnumerator GetBadWords(Action<string> callback)
    {
        string url = APIurl;
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            yield return www.downloadHandler.text;
            callback(www.downloadHandler.text);
        }
    }
}

[Serializable]
public class Message
{
    public string text;
    public Text textObject;
    public MessageType messageType;

    public enum MessageType
    {
        playerMessage,
        info
    }
}

[Serializable]
public class BadWords
{
    public string[] data;
}

public static class ReplaceBadWords
{
    /// <summary>
    /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another 
    /// specified string according the type of search to use for the specified string.
    /// </summary>
    /// <param name="str">The string performing the replace method.</param>
    /// <param name="oldValue">The string to be replaced.</param>
    /// <param name="newValue">The string replace all occurrences of <paramref name="oldValue"/>. 
    /// If value is equal to <c>null</c>, than all occurrences of <paramref name="oldValue"/> will be removed from the <paramref name="str"/>.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="oldValue"/> are replaced with <paramref name="newValue"/>. 
    /// If <paramref name="oldValue"/> is not found in the current instance, the method returns the current instance unchanged.</returns>
    public static string ReplaceCaseInsensitive(this string str, string oldValue, string @newValue, StringComparison comparisonType)
    {
        // Check inputs.
        if (str == null)
        {
            // Same as original .NET C# string.Replace behavior.
            throw new ArgumentNullException(str);
        }
        if (str.Length == 0)
        {
            // Same as original .NET C# string.Replace behavior.
            return str;
        }
        if (oldValue == null)
        {
            // Same as original .NET C# string.Replace behavior.
            throw new ArgumentNullException(oldValue);
        }
        if (oldValue.Length == 0)
        {
            // Same as original .NET C# string.Replace behavior.
            throw new ArgumentException("String cannot be of zero length.");
        }

        //if (oldValue.Equals(newValue, comparisonType))
        //{
        //This condition has no sense
        //It will prevent method from replacesing: "Example", "ExAmPlE", "EXAMPLE" to "example"
        //return str;
        //}

        // Prepare string builder for storing the processed string.
        // Note: StringBuilder has a better performance than String by 30-40%.
        StringBuilder resultStringBuilder = new StringBuilder(str.Length);

        // Analyze the replacement: replace or remove.
        bool isReplacementNullOrEmpty = string.IsNullOrEmpty(@newValue);

        // Replace all values.
        const int valueNotFound = -1;
        int foundAt;
        int startSearchFromIndex = 0;
        while ((foundAt = str.IndexOf(oldValue, startSearchFromIndex, comparisonType)) != valueNotFound)
        {
            // Append all characters until the found replacement.
            int @charsUntilReplacment = foundAt - startSearchFromIndex;
            bool isNothingToAppend = @charsUntilReplacment == 0;
            if (!isNothingToAppend)
            {
                resultStringBuilder.Append(str, startSearchFromIndex, @charsUntilReplacment);
            }
            // Process the replacement.
            if (!isReplacementNullOrEmpty)
            {
                resultStringBuilder.Append(@newValue);
            }
            // Prepare start index for the next search.
            // This needed to prevent infinite loop, otherwise method always start search 
            // from the start of the string. For example: if an oldValue == "EXAMPLE", newValue == "example"
            // and comparisonType == "any ignore case" will conquer to replacing:
            // "EXAMPLE" to "example" to "example" to "example" … infinite loop.
            startSearchFromIndex = foundAt + oldValue.Length;
            if (startSearchFromIndex == str.Length)
            {
                // It is end of the input string: no more space for the next search.
                // The input string ends with a value that has already been replaced. 
                // Therefore, the string builder with the result is complete and no further action is required.
                return resultStringBuilder.ToString();
            }
        }
        // Append the last part to the result.
        int @charsUntilStringEnd = str.Length - startSearchFromIndex;
        resultStringBuilder.Append(str, startSearchFromIndex, @charsUntilStringEnd);
        return resultStringBuilder.ToString();
    }
}