/*
 * LobbyPlayer.cs
 * Authors: Christian
 * Description: Supplies a player controller for the lobby, that allows users to change their own settings but not 
 * other player's settings.
 */
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyPlayer : NetworkLobbyPlayer {

    private Lobby lob;
    private GameSettings settings;

    [SyncVar(hook = "CallUpdateTeam")]
    private int playerTeam = 0;

    [SyncVar(hook = "CallUpdateName")]
    private string playerName;

    void Update() {
        //TEMP?
        //Listen for input to change team or ready up.
        if (SceneManager.GetActiveScene().name == "Pregame Menu") {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("LB")) {
                lob.ChangeTeam(this.slot);
            }
            if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("RB")) {
                lob.onClickReady(this.slot);
            }
        }
    }

    [Command]
    public void CmdChangeTeam() {

        if (playerTeam != 3) {
            playerTeam++;
        } else {
            playerTeam = 0;
        }
    }

    [Command]
    public void CmdChangeName(string pn) {
        playerName = pn;
    }

    private void CallUpdateTeam(int pt) {
        GameObject.Find("GameLobby").GetComponent<Lobby>().UpdateTeam(pt, this.slot);
    }

    private void CallUpdateName(string name) {
        GameObject.Find("GameLobby").GetComponent<Lobby>().UpdateName(name, this.slot);
    }

    void Awake() {
        //wait a frame to setup stuff because unity sucks
        StartCoroutine("Setup");
    }

    //Give the player's slot a reference to the lobbyplayer
    IEnumerator Setup() {
        yield return new WaitForFixedUpdate();
        lob = GameObject.Find("GameLobby").GetComponent<Lobby>();
        CallUpdateTeam(this.playerTeam);
        if (isLocalPlayer) {
            settings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
            lob.setPlayer(this);
            CmdChangeName(settings.getLocalPlayerName());
        } else {
            CallUpdateName(playerName);
        }
    }

    //hook called when player sets to ready
    public override void OnClientReady(bool readyState) {
        lob.ChangeReadyColour(readyState, slot);
    }

    public int GetTeam() {
        return this.playerTeam;
    }

    public string getPlayerName() {
        return playerName;
    }
}
