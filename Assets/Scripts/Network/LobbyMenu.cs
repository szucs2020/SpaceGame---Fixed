/*
 * LobbyMenu.cs
 * Authors: Christian
 * Description: A UI controlled class that starts a server or joins a game when buttons are clicked
 */
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyMenu : MonoBehaviour {

    private CustomNetworkLobby lobbyManager;
    public InputField ipAddress;
    public GameObject lobby;
    public GameObject load;

    public GameSettings settings;

    void Awake() {
        Object gs = Instantiate(Resources.Load("GameSettings"), new Vector3(0, 0, 0), Quaternion.identity);
        gs.name = "GameSettings";
        GameObject gObjSet = (GameObject)gs;
        settings = gObjSet.GetComponent<GameSettings>();
    }

    void Start() {

        Object g = Instantiate(Resources.Load("NetworkManager"), new Vector3(0, 0, 0), Quaternion.identity);
        g.name = "NetworkManager";
        lobbyManager = ((GameObject)g).GetComponent<CustomNetworkLobby>();

        Audio2D.singleton.StopSound("GameMusic");
        //Audio2D.singleton.PlaySound("MenuMusic");

        InputField name = GameObject.Find("Menu").gameObject.transform.Find("PlayerName").GetComponent<InputField>();
        NameChanged(name.text);

        StartCoroutine("InitSelect");
    }

    void OnAwake() {
        StartCoroutine("InitSelect");
    }

    //Highlight first option for controller-friendly menu.
    public IEnumerator InitSelect() {
        GameObject panel = GameObject.Find("Menu").gameObject;
        Button host = panel.transform.Find("Host Game").gameObject.GetComponent<Button>();

        EventSystem.current.SetSelectedGameObject(null);
        yield return null;
        EventSystem.current.SetSelectedGameObject(host.gameObject);
    }

    public void onClickHost() {
        GameObject panel = GameObject.Find("Menu").gameObject;
        InputField name = panel.transform.Find("PlayerName").GetComponent<InputField>();
        Text error = panel.transform.Find("Error").GetComponent<Text>();

        if (name.text == "") {
            error.text = "PLEASE ENTER A NAME.";
        } else {
            error.text = "";

            lobbyManager.HostGame();
            this.gameObject.SetActive(false);
            lobby.SetActive(true);
        }
    }

    public void onClickJoin() {
        GameObject panel = GameObject.Find("Menu").gameObject;
        InputField name = panel.transform.Find("PlayerName").GetComponent<InputField>();
        InputField ip = panel.transform.Find("IPAddress").GetComponent<InputField>();
        Text error = panel.transform.Find("Error").GetComponent<Text>();

        if (name.text == "") {
            error.text = "PLEASE ENTER A NAME.";
        } else if (ip.text == "") {
            error.text = "PLEASE ENTER AN IP ADDRESS.";
        } else {
            error.text = "";

            lobbyManager.JoinGame(ipAddress.text);
            this.gameObject.SetActive(false);
            load.SetActive(true);
        }
    }

    public void NameChanged(string s) {
        settings.setLocalPlayerName(s);
    }
}
