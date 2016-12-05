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

    void Start() {

        Object g = Instantiate(Resources.Load("NetworkManager"), new Vector3(0, 0, 0), Quaternion.identity);
        g.name = "NetworkManager";
        lobbyManager = ((GameObject)g).GetComponent<CustomNetworkLobby>();

        if (GameObject.Find("Audio") == null) {
            Object audio = Instantiate(Resources.Load("Audio/Audio"), new Vector3(0, 0, 0), Quaternion.identity);
            audio.name = "Audio";
        }

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
        lobbyManager.HostGame();
        this.gameObject.SetActive(false);
        lobby.SetActive(true);
    }

    public void onClickJoin() {
        lobbyManager.JoinGame(ipAddress.text);
        this.gameObject.SetActive(false);
        load.SetActive(true);
    }
}
