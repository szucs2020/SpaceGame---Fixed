/*
 * LetterBox.cs
 * Authors: Christian
 * Description: This script adds a letterbox and pillarbox to adjust the screen size for different resolutions
 */
using UnityEngine;
using System.Collections;

public class LetterBox : MonoBehaviour {

    Camera mainCamera;

	// Use this for initialization
	void Start () {
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
}
