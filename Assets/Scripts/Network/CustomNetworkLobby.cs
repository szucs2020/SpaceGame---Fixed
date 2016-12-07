/*
 * CustomNetworkLobby.cs
 * Authors: Christian
 * Description: This script extends the functionality of the network lobby manager class, and manages the client instances
 */
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.Match;

public class CustomNetworkLobby : NetworkLobbyManager {

    public float timeout;
    private GameObject menu;
    private GameObject load;
    private GameObject lobby;

    private bool sneakyFlag = false;
    private NetworkClient localClient;
    private bool connecting = false;
    private float timeLeft;
    private bool isHost = false;

    private bool stopClient;

    void Start() {
        menu = GameObject.Find("Menu");
        load = menu.GetComponent<LobbyMenu>().load;
        lobby = menu.GetComponent<LobbyMenu>().lobby;
    }

    public void HostGame() {
        localClient = StartHost();
        isHost = true;
    }

    void Update() {
        if (connecting) {

            if (localClient != null && localClient.isConnected) {

                connecting = false;
                load.SetActive(false);
                lobby.SetActive(true);

            } else {
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0.0f) {
                    connecting = false;
                    localClient.Shutdown();
                    load.SetActive(false);
                    menu.SetActive(true);
                }
            }
        }
    }

    public override void OnStopClient() {
        Destroy(this.gameObject);
        SceneManager.LoadScene("EndGame");
    }

    public void JoinGame(String ipAddress) {

        networkAddress = ipAddress;
        localClient = StartClient();

        timeLeft = timeout;
        connecting = true;
    }

    public void CloseConnection() {
        Shutdown();
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer) {
        Player p = gamePlayer.GetComponent<Player>();
        LobbyPlayer lp = lobbyPlayer.GetComponent<LobbyPlayer>();
        p.playerSlot = lp.slot;
        p.playerName = lp.getPlayerName();
        return true;
    }

    //getting around the fact that this is called twice every time someone joins
    public override void OnServerConnect(NetworkConnection conn) {
        if (sneakyFlag) {
            this.minPlayers++;
        } else {
            sneakyFlag = true;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        this.minPlayers--;
    }

    public override void OnStopServer() {
        if (!isHost) {
            this.StopClient();
            Destroy(this);
        }
    }

    public bool getIsHost() {
        return isHost;
    }
}
