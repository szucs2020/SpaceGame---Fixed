/*
 * Network.cs
 * Authors: Nigel
 * Description: Handles scene transitions and the in-game pause menu.
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

    //Highlight first option for controller-friendly menu.
    public IEnumerator InitSelect() {
        if (pauseMenu) {
            GameObject panel = GameObject.Find("Pause Menu").gameObject;
            Button host = panel.transform.Find("Menu Button").gameObject.GetComponent<Button>();

            EventSystem.current.SetSelectedGameObject(null);
            yield return null;
            EventSystem.current.SetSelectedGameObject(host.gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu) {
            if (!active) {
                StartCoroutine("InitSelect");
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

    public void BackToMenu() {
        //TODO: Write in disconnect shit.


        LoadSceneMode mode = LoadSceneMode.Single;
        SceneManager.LoadSceneAsync("Pregame Menu", mode);
    }

    public void ExitGame() {
        //TODO: Write disconnect shit.

        Application.Quit();
    }
}
