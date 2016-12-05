/*
 * Timer.cs
 * Authors: Christian
 * Description: This script controls the timer for the "Time" gametype. It counts down and displays only when 
 * its changed by one full second.
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    private float timeLeft;
    private bool timeSet = false;
    int lastTime;
    private Text clock;

    // Update is called once per frame
    void Update() {
        if (timeSet) {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0.0f) {
                timeSet = false;
                GameObject.Find("GameSettings").GetComponent<GameController>().EndGame();
            } else {
                if (lastTime != (int)timeLeft) {
                    UpdateClock();
                }
            }
        }
    }

    //updates the UI text
    private void UpdateClock() {
        if (clock == null) {
            print("clock is null"); ;
        }
        clock.text = CreateTime((int)timeLeft);
        lastTime = (int)timeLeft;
    }

    //creates a timer string given a number of seconds
    private string CreateTime(int t) {

        int minutes = t / 60;
        int seconds = t % 60;

        string sMinutes;
        string sSeconds;

        //make sure the numbers have two digits
        if (seconds < 10) {
            sSeconds = "0" + seconds.ToString();
        } else {
            sSeconds = seconds.ToString();
        }

        if (minutes < 10) {
            sMinutes = "0" + minutes.ToString();
        } else {
            sMinutes = minutes.ToString();
        }

        return sMinutes + ":" + sSeconds;
    }

    public void setTime(float t) {
        clock = GetComponent<Text>();
        timeLeft = t;
        UpdateClock();
        timeSet = true;
    }
}
