/*
 * CameraExpander.cs
 * Authors: Christian
 * Description: This script moves the camera and changes its size to show all the players on the screen at once.
 */
using System;
using UnityEngine;

public class CameraExpander : MonoBehaviour {

    private const float moveSpeed = 2.0f;
    private const float zoomSpeed = 10.0f;
    private const int minCameraSize = 70;
    private const int margin = 30;
    public Vector3 offset = new Vector3(0f, 0f, -1f);

    private GameObject[] players;
    private Camera mainCamera;

    void Start() {

        //get current ratio and get the amount needed to scale by to reach the target aspect ratio
        float gameAspect = 16.0f / 9.0f;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / gameAspect;
        mainCamera = GetComponent<Camera>();

        //add letterbox and pillarbox to adjust screen
        if (scaleHeight < 1.0f) {

            Rect letterbox = mainCamera.rect;
            letterbox.width = 1.0f;
            letterbox.height = scaleHeight;
            letterbox.x = 0;
            letterbox.y = (1.0f - scaleHeight) / 2.0f;
            mainCamera.rect = letterbox;
        } else {
            float scalewidth = 1.0f / scaleHeight;

            Rect pillarbox = mainCamera.rect;
            pillarbox.width = scalewidth;
            pillarbox.height = 1.0f;
            pillarbox.x = (1.0f - scalewidth) / 2.0f;
            pillarbox.y = 0;
            mainCamera.rect = pillarbox;
        }
    }

    private void LateUpdate(){

        float centreX = 0.0f;
        float centreY = 0.0f;
        float lowestX = float.MaxValue;
        float highestX = float.MinValue;
        float lowestY = float.MaxValue;
        float highestY = float.MinValue;
        float camTarget;

        players = GameObject.FindGameObjectsWithTag("player");

        if (players != null && players.Length > 0) {
            for (int i = 0; i < players.Length; i++) {

                //get centre point on screen
                centreX += players[i].transform.position.x;
                centreY += players[i].transform.position.y;

                //get highest and lowest x values
                if (players[i].transform.position.x < lowestX) {
                    lowestX = players[i].transform.position.x;
                } else if (players[i].transform.position.x > highestX) {
                    highestX = players[i].transform.position.x;
                }

                //get highest and lowest y values
                if (players[i].transform.position.y < lowestY) {
                    lowestY = players[i].transform.position.y;
                } else if (players[i].transform.position.y > highestY) {
                    highestY = players[i].transform.position.y;
                }
            }

            centreX /= players.Length;
            centreY /= players.Length;

            //set the size to include all the players
            if ((highestX - lowestX) > (highestY - lowestY)) {
				camTarget = ((highestX - lowestX) * 0.51f) + margin;
            } else {
				camTarget = ((highestY - lowestY) * 0.51f) + margin;
            }
            
            if (camTarget < minCameraSize) {
                camTarget = minCameraSize;
            }

            //update camera size and position
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, camTarget, zoomSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, new Vector3(centreX, centreY, -10), moveSpeed * Time.deltaTime);
        }
    }
}
