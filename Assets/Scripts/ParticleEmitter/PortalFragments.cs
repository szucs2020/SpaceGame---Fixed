/*
 * PortalFragments.cs
 * Authors: Lorant
 * Description: This script controls the functionality
 *              of the inside of the portals
 */
using UnityEngine;
using System.Collections;

public class PortalFragments : Particle {
    public float size;
    public bool changeSize = true;

	// Use this for initialization
	void Start () {
        minSize = Random.Range(size-0.2f, size);
        maxSize = size;
        deltaSizeRate = 0.2f;
        startTime = Time.time - 5;
        minRed = 0;
        maxRed = 150;
        minBlue = 200;
        maxBlue = 255;
        minGreen = 50;
        maxGreen = 150;
        deltaTC = Random.Range(1.5f, 4f);
        Color a = Color.cyan;
        Color b = Color.blue;

        radius = 1;

        sprite = GetComponent<SpriteRenderer>();
        sprite.material.SetColor("_Color", b);
    }
	
	// Update is called once per frame
	void Update () {
        changeParticleColour(sprite);
        if (changeSize)
        {
            changeParticleSize();
        }
    }
}
