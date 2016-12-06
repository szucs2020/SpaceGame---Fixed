/*
 * Chat.cs
 * Authors: Lorant
 * Description: This script allows player messaging over the network
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Chat : NetworkBehaviour
{
    public Text chatText;
    public InputField chatInput;

    public static Chat singleton = null;

    void Awake()
    {
        if (singleton != null && singleton != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            singleton = this;
        }
        DontDestroyOnLoad(this);
    }

    public void PrintMessage(string message)
    {
        chatText.text += message;
    }

}
