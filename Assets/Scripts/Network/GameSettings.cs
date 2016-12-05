/*
 * GameSettings.cs
 * Authors: Christian
 * Description: Allows us to save configuration settings, and move them from the lobby scene to the gameplay scene
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameSettings : NetworkBehaviour {

    //possible game types
    public enum GameType {
        Survival = 1,
        Time = 2
    }

    //settings
    public GameType gameType;
    public int numLives;
    public int time;
    public int NumberOfAIPlayers;

    private string localPlayerName;

    void Start() {

        // prevent the scene from destroying this object
        DontDestroyOnLoad(transform.gameObject);

        //default values
        gameType = GameType.Survival;
        NumberOfAIPlayers = 0;
        time = 120;
        numLives = 3;
    }

    void OnLevelWasLoaded(int level) {

        //gameplay scene
        if (level == SceneManager.GetSceneByName("Main").buildIndex) {
            GetComponent<GameController>().StartGame();
            gameObject.GetComponent<GameController>().SpawnAllAI();
        }
    }

    public void setLocalPlayerName(string pn) {
        localPlayerName = pn;
    }
    public string getLocalPlayerName() {
        return this.localPlayerName;
    }
}
