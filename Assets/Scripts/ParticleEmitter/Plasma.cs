/*
 * Plasma.cs
 * Authors: Lorant
 * Description: This script controls the functionality of the
 *              plasma particle.  Plasma particles are used in the plasm ball
 */
using UnityEngine;
using System.Collections;

public class Plasma : Particle {

	// Use this for initialization
	void Start () {
        this.type = ParticleTypes.Plasma;

        lifeSpan = 5f;
        tAlive = 0;
        minSize = Random.Range(0.01f, 0.02f);
        maxSize = Random.Range(minSize + 0.01f, minSize + 0.03f);
        deltaSizeRate = 0.02f;
        startTime = Time.time - 5;
        minRed = 0;
        maxRed = 200;
        minBlue = 200;
        maxBlue = 255;
        minGreen = 50;
        maxGreen = 150;
        deltaTC = Random.Range(0.5f, 2f);
        Color a = Color.cyan;
        Color b = Color.blue;

        radius = 1;

        sprite = GetComponent<SpriteRenderer>();
        sprite.material.SetColor("_Color", new Color(0, 0, 0, 0));
        destroy();
    }
	
	// Update is called once per frame
	void Update () {
        this.controlPlasma();
    }

    private void controlPlasma()
    {
        changeParticleColour(sprite);
        changeParticleSize();
        rotateParticle();
    }

    private void destroy()
    {
        if (this.transform.parent != null)
        {
            Destroy(this.transform.parent.gameObject, lifeSpan);
        }
    }
}
