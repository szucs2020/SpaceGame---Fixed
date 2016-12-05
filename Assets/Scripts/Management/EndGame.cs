using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {

    // Use this for initialization
    void Start() {
        GameObject g = GameObject.Find("Audio");
        if (g != null) {
            Destroy(g);
        }
        SceneManager.LoadScene("Pregame Menu");
    }
}
