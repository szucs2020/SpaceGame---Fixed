/*
 * Network.cs
 * Authors: Nigel
 * Description: Handles scene transitions and the in-game pause menu.
 */
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour {
    CanvasGroup menu;
    bool active = false;
    public bool pauseMenu;

    // Use this for initialization
    void Start() {
        if (pauseMenu) {
            menu = this.gameObject.GetComponent<CanvasGroup>();
            menu.alpha = 0;
            menu.interactable = false;
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu) {
            if (!active) {
                active = true;
                menu.alpha = 1;
                menu.interactable = true;
            } else {
                active = false;
                menu.alpha = 0;
                menu.interactable = false;
            }
        }
    }

    public void LoadScene(string scene) {
        LoadSceneMode mode = LoadSceneMode.Single;
        SceneManager.LoadSceneAsync(scene, mode);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
