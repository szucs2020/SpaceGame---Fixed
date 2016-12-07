using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour {

    private GameSettings settings;
    private GameController controller;
    private CustomNetworkLobby manager;
    private Text winner;
    private Text scoreboard;

    void Awake() {
        manager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkLobby>();
        settings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
        controller = GameObject.Find("GameSettings").GetComponent<GameController>();
        winner = GameObject.Find("Menu").transform.Find("Winner").GetComponent<Text>();
        scoreboard = GameObject.Find("Menu").transform.Find("ScoreBoard").GetComponent<Text>();
    }

    // Use this for initialization
    void Start() {

        if (settings.gameType == GameSettings.GameType.Survival) {
            int winningPlayer = 0;
            for (int i = 0; i < controller.playerPoints.Length; i++) {
                if (controller.playerLives[i] > 0) {
                    winningPlayer = i;
                }
            }
            winner.text = "Player " + (winningPlayer + 1) + " wins!";
        } else {
            int winningPlayer = 0;
            for (int i = 0; i < controller.playerPoints.Length; i++) {
                if (controller.playerPoints[i] > controller.playerPoints[winningPlayer]) {
                    winningPlayer = i;
                }
            }
            winner.text = "Player " + (winningPlayer + 1) + " wins!";

            int numHighest = 0;
            for (int i = 0; i < controller.playerPoints.Length; i++) {
                if (controller.playerPoints[winningPlayer] == controller.playerPoints[i]) {
                    numHighest++;
                }
            }

            //its a tie
            if (numHighest > 1) {
                winner.text = "It's a tie!";
            }
        }

        //scoreboard
        for (int i = 0; i < controller.playerPoints.Length; i++) {
            scoreboard.text += "Player " + (i + 1) + " : " + controller.playerPoints[i] + "\n";
        }
        Destroy(settings.gameObject);
    }

    public void OK() {
        manager.CloseConnection();
    }
}
