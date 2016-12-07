using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EndGame : NetworkBehaviour {

    private GameSettings settings;
    private GameController controller;
    private CustomNetworkLobby manager;
    private Text winner;
    private Text scoreboard;

    [SyncVar (hook = "winnerChanged")]
    private string winnerText = "";
    [SyncVar(hook = "scoreChanged")]
    private string scoreText = "";

    void Awake() {
        manager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkLobby>();
        settings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
        controller = GameObject.Find("GameSettings").GetComponent<GameController>();
        winner = GameObject.Find("Menu").transform.Find("Winner").GetComponent<Text>();
        scoreboard = GameObject.Find("Menu").transform.Find("ScoreBoard").GetComponent<Text>();
    }

    // Use this for initialization
    void Start() {

        if (isServer) {

            string tempWinner = "";
            string tempScore = "";

            if (settings.gameType == GameSettings.GameType.Survival) {
                int winningPlayer = 0;
                for (int i = 0; i < controller.playerPoints.Length; i++) {
                    if (controller.playerLives[i] > 0) {
                        winningPlayer = i;
                    }
                }
                tempWinner = "Player " + (winningPlayer + 1) + " wins!";
            } else {
                int winningPlayer = 0;
                for (int i = 0; i < controller.playerPoints.Length; i++) {
                    if (controller.playerPoints[i] > controller.playerPoints[winningPlayer]) {
                        winningPlayer = i;
                    }
                }
                tempWinner = "Player " + (winningPlayer + 1) + " wins!";

                int numHighest = 0;
                for (int i = 0; i < controller.playerPoints.Length; i++) {
                    if (controller.playerPoints[winningPlayer] == controller.playerPoints[i]) {
                        numHighest++;
                    }
                }

                //its a tie
                if (numHighest > 1) {
                    tempWinner = "It's a tie!";
                }
            }
            winnerText = tempWinner;

            //scoreboard
            for (int i = 0; i < controller.playerPoints.Length; i++) {
                tempScore += "Player " + (i + 1) + " : " + controller.playerPoints[i] + "\n";
            }
            scoreText = tempScore;
        }
    }

    private void winnerChanged(string w) {
        winner.text = w;
    }

    private void scoreChanged(string s) {
        scoreboard.text = s;
    }

    public void OK() {
        Destroy(settings.gameObject);
        manager.CloseConnection();
        SceneManager.LoadScene("Pregame Menu");
    }
}
