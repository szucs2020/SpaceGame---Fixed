/*
 * GameController.cs
 * Authors: Christian
 * Description: This script controls the gametypes and respawning of the players
 */
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour {

    private CustomNetworkLobby manager;
    private GameSettings settings;
    public int[] playerLives;
    public int[] playerPoints;

    [SyncVar]
    private int numberOfPlayers;

    [SyncVar]
    private int time;

    [SyncVar(hook = "GameStarted")]
    private int started = 0;

    [SerializeField]
    GameObject AIPrefab;

    void Start() {
        settings = GetComponent<GameSettings>();
        manager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkLobby>();
    }

    private void GameStarted(int s) {
        if (s == 1) {

            if (settings.gameType == GameSettings.GameType.Survival) {
                playerLives = new int[numberOfPlayers];
                for (int i = 0; i < playerLives.Length; i++) {
                    playerLives[i] = settings.numLives;
                }
                
            } else if (settings.gameType == GameSettings.GameType.Time) {
                GameObject.Find("HUD").transform.Find("Timer").GetComponent<Timer>().setTime(settings.time);
            }

            //create and initialize score array
            playerPoints = new int[numberOfPlayers];
            for (int i = 0; i < playerPoints.Length; i++) {
                playerPoints[i] = 0;
            }

            SpawnAllAI();
        }
    }

    public void StartGame() {
        if (isServer) {
            numberOfPlayers = settings.NumberOfAIPlayers + NetworkManager.singleton.numPlayers;
            time = settings.time;
            started = 1;
        }
    }

    [Command]
    private void CmdStartGame() {
        numberOfPlayers = settings.NumberOfAIPlayers + NetworkManager.singleton.numPlayers;
        time = settings.time;
        started = 1;
    }

    public void EndGame() {
        StartCoroutine("DelayEnd");
    }

    IEnumerator DelayEnd() {
        yield return new WaitForSeconds(0.5f);
        manager.ServerChangeScene("EndGame");
    }

    public void AttemptSpawnPlayer(NetworkConnection connectionToClient, short playerControllerID, int playerSlot, string playerName, int killer) {

        if (killer == playerSlot || killer == -1) {
            playerPoints[playerSlot]--;
        } else {
            playerPoints[killer]++;
        }

        bool respawn = false;
        bool end = false;

        if (settings.gameType == GameSettings.GameType.Survival) {

            playerLives[playerSlot]--;

            //check if there is a winner
            end = isGameOver();
            if (end) {
                EndGame();
            }

            if (playerLives[playerSlot] > 0) {
                respawn = true;
            }
        }

        if (end == false && (respawn == true || settings.gameType == GameSettings.GameType.Time)) {
            Transform spawn = NetworkManager.singleton.GetStartPosition();
            GameObject newPlayer = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, spawn.position, spawn.rotation);
            newPlayer.GetComponent<Player>().playerSlot = playerSlot;
            newPlayer.GetComponent<Player>().playerName = playerName;
            NetworkServer.ReplacePlayerForConnection(connectionToClient, newPlayer, playerControllerID);
        }
    }

    private bool isGameOver() {

        int playersLeft = 0;
        for (int i = 0; i < playerLives.Length; i++) {
            if (playerLives[i] > 0) {
                playersLeft++;
            }
        }
        if (playersLeft > 1) {
            return false;
        } else {
            return true;
        }
    }

    // Update is called once per frame
    public void SpawnAllAI() {
        if (!isServer) {
            return;
        }
        for (int i = NetworkManager.singleton.numPlayers; i < numberOfPlayers; i++) {
            SpawnAI(i, "Michael Wirth");
        }
    }

    public void AttemptSpawnAI(int playerSlot, string name, int killer) {

        bool end = false;
        bool respawn = false;

        if (killer == playerSlot || killer == -1) {
            playerPoints[playerSlot]--;
        } else {
            playerPoints[killer]++;
        }

        if (settings.gameType == GameSettings.GameType.Survival) {

            playerLives[playerSlot]--;

            //check if there is a winner
            end = isGameOver();
            if (end) {
                EndGame();
            }

            if (playerLives[playerSlot] > 0) {
                respawn = true;
            }
        }

        if (end == false && (respawn == true || settings.gameType == GameSettings.GameType.Time)) {
            SpawnAI(playerSlot, name);
        }
    }

    private void SpawnAI(int slot, string name) {
        Transform t = manager.GetStartPosition();
        GameObject AI = (GameObject)GameObject.Instantiate(AIPrefab, t.position, Quaternion.identity);
        Player p = AI.GetComponent<Player>();
        p.playerSlot = slot;
        p.playerName = name;
        NetworkServer.Spawn(AI);
    }
}
