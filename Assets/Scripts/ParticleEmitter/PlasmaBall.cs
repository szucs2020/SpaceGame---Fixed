/*
 * PlasamBall.cs
 * Authors: Lorant
 * Description: This script controls the functionality of the 
 *              palsma ball, which is used as ammo for the
 *              plsam cannon
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlasmaBall : MonoBehaviour {

    List<Particle> particles;
    public int amount;
    public float radius;
    public Plasma plasma;
    public Player bulletOwner;
    private bool hurtSelf = false;
    private bool varDestroy = false;
    private float destroyTime = 0.3f;
    private float tAtDestroy = 0;
    private float timePassed = 0;

    // Use this for initialization
    void Start () {

        particles = new List<Particle>();

        for (int i = 0; i < amount - 1; i++) {

            //create particles
            particles.Add((Particle)Instantiate(plasma, transform.position, Quaternion.identity));

            particles[i].transform.SetParent(transform, false);
            particles[i].transform.localPosition = new Vector3(0, 0, 0);

            //place particles
            Vector3 newPosition = Quaternion.AngleAxis(Mathf.Acos(Random.Range(0f, 2 * Mathf.PI)), particles[i].transform.localPosition)
                                     * new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), 0f);
            particles[i].transform.localPosition = newPosition;
        }
    }

    void Update()
    {
        if (varDestroy)
        {
            destroyAnim();
        }
    }

    void destroyAnim()
    {
        timePassed = 1 -(Time.time - tAtDestroy);
        for (int i = 0; i < amount-1; i++)
        {
            if (timePassed < 0.05)
            {
                particles[i].transform.Translate((particles[i].transform.position - particles[i].transform.parent.localPosition) * (timePassed/2 + timePassed*-3));
            }
            else
            {
                particles[i].transform.Translate((particles[i].transform.position - particles[i].transform.parent.localPosition) * (timePassed/5));
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Ground")
        {
            tAtDestroy = Time.time;
            varDestroy = true;
            Destroy(gameObject, destroyTime);
        }
        else if (col.gameObject.tag == "Portal")
        {
            hurtSelf = true;
        }
        else if (LayerMask.LayerToName(col.gameObject.layer) == "Player")
        {
            if (col.gameObject.GetComponent<Player>().netId != bulletOwner.netId || hurtSelf == true)
            {
                tAtDestroy = Time.time;
                varDestroy = true;
                Destroy(gameObject, destroyTime);
                col.gameObject.GetComponent<Health>().Damage(10.0f);
            }
        }
    }
}
