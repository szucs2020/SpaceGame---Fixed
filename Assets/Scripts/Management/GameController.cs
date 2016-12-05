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
    private int[] playerLives;

    [SerializeField]
    GameObject AIPrefab;

    void Start() {
        settings = GetComponent<GameSettings>();
        manager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkLobby>();
    }

    public void StartGame() {

        if (settings.gameType == GameSettings.GameType.Survival) {

            //Setup player lives
            playerLives = new int[settings.NumberOfAIPlayers + NetworkManager.singleton.numPlayers];
            for (int i = 0; i < playerLives.Length; i++) {
                playerLives[i] = settings.numLives;
            }

        } else if (settings.gameType == GameSettings.GameType.Time) {
            GameObject.Find("HUD").transform.Find("Timer").GetComponent<Timer>().setTime(settings.time);
        }
    }

    public void EndGame() {
        manager.CloseConnection();
        Destroy(manager.gameObject);
        Destroy(settings.gameObject);
        SceneManager.LoadScene("EndGame");
    }

    public void AttemptSpawnPlayer(NetworkConnection connectionToClient, short playerControllerID, int playerSlot, string playerName) {

        bool respawn = false;
        bool end = false;

        print("slot: " + playerSlot);
        print("lives length: " + playerLives.Length);

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
        for (int i = NetworkManager.singleton.numPlayers; i < playerLives.Length; i++) {
            SpawnAI(i, "Michael Wirth");
        }
    }

    public void AttemptSpawnAI(int slot, string name) {

        bool end = false;
        bool respawn = false;

        if (settings.gameType == GameSettings.GameType.Survival) {

            playerLives[slot]--;

            //check if there is a winner
            end = isGameOver();
            if (end) {
                EndGame();
            }

            if (playerLives[slot] > 0) {
                respawn = true;
            }
        }

        if (end == false && (respawn == true || settings.gameType == GameSettings.GameType.Time)) {
            SpawnAI(slot, name);
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
