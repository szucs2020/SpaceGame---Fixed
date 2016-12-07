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

    public static GameSettings singleton;

    //possible game types
    public enum GameType {
        Survival = 1,
        Time = 2
    }

    //settings
    [SyncVar]
    public GameType gameType;

    [SyncVar]
    public int numLives;

    [SyncVar]
    public int time;

    [SyncVar]
    public int NumberOfAIPlayers;
    private string localPlayerName = "Player";
    public bool playMusic = true;

    void Awake() {
        singleton = this;
    }

    void Start() {

        // prevent the scene from destroying this object
        DontDestroyOnLoad(transform.gameObject);

        //default values
        if (isServer) {
            gameType = GameType.Survival;
            NumberOfAIPlayers = 0;
            time = 120;
            numLives = 3;
        }
    }

    void OnLevelWasLoaded(int level) {

        //gameplay scene
        if (level == SceneManager.GetSceneByName("Main").buildIndex) {
            GetComponent<GameController>().StartGame();
            if (playMusic) {
                Audio2D.singleton.StopSound("MenuMusic");
                Audio2D.singleton.PlaySound("GameMusic");
            }
        }
    }

    public void setLocalPlayerName(string pn) {
        localPlayerName = pn;
    }
    public string getLocalPlayerName() {
        return this.localPlayerName;
    }

    public void setPlayMusic(bool p) {
        playMusic = p;
        if (p) {
            Audio2D.singleton.PlaySound("MenuMusic");
        } else {
            Audio2D.singleton.StopSound("MenuMusic");
        }
    }
}
