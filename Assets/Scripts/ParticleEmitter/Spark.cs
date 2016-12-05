/*
 * Spark.cs
 * Authors: Lorant
 * Description: This script spawns a spark when a
 *              laser collides with an object
 */
using UnityEngine;
using System.Collections;

public class Spark : MonoBehaviour {
    private float startTime;
    private float timePassed;


	// Use this for initialization
	void Start () {
        startTime = Time.time;
        Destroy(gameObject, 0.3f);
	}
	
	// Update is called once per frame
	void Update () {
        timePassed = 1-( Time.time - startTime);
        transform.localScale = transform.localScale * timePassed;
	}
}
