using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {

    // Use this for initialization
    void Start() {
        Audio2D.singleton.StopSound("GameMusic");
    }
}
