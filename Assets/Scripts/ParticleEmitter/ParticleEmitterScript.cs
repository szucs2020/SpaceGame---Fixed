/*
 * ParticleEmitter.cs
 * Authors: Lorant
 * Description: This script is obselete
 *              It was original used to emit the different particle
 *              types before I pivoted approaches
 */
using UnityEngine;
using System.Collections.Generic;

public class ParticleEmitterScript : MonoBehaviour {

    float xPos;
    float ypos;
    float angle;
    float velocity;
    float rate;
    float dt;
    float prevTime;
    float lifeSpan;
    float amount;
    float radius;
    int amountExists;
    Particle.ParticleTypes particleType;

    List<Particle> particles;

    public LaserDot laserDot;
    public Plasma plasma;
    public GameObject plasmaAnchor;

    // Use this for initialization
    void Start () {

    }
	
    
	// Update is called once per frame
	void Update () {
    }

    public LaserDot[] GenerateShotgunShells(Vector2 direction, Vector2 position)
    {
        LaserDot[] lasers = new LaserDot[5];
        Vector2 newDir;

        for (int i = 0; i < 5; i++)
        {
            newDir = Quaternion.AngleAxis(Random.Range(-10, 10), Vector3.back) * direction;
            lasers[i] = (LaserDot)CreateParticle((Particle)laserDot, position);
            //lasers[i].GetComponent<Rigidbody2D>().velocity = velocity * newDir;
        }

        return lasers;
    }

    public GameObject GeneratePlasma(Vector2 position)
    {
        amount = 50;
        radius = 2;

        GameObject centrePoint = (GameObject)Instantiate(plasmaAnchor, position, Quaternion.identity);

        //CreateParticles((int)amount, centrePoint);
        //placeParticles();

        return centrePoint;
    }

    public LaserDot GenerateLaserDot(Vector2 position)
    {
        LaserDot laser;

        laser = (LaserDot)CreateParticle((Particle)laserDot, position);

        return laser;
    }

    private Particle CreateParticle(Particle particle, Vector2 position)
    {
        return (Particle)Instantiate(particle, position, Quaternion.identity);
    }

    //private void CreateParticles(int amount, GameObject anchor)
    //{
    //    particles = new List<Particle>();

    //    for (int i = 0; i < amount - 1; i++)
    //    {
    //        particles.Add((Particle)Instantiate(plasma, transform.position, Quaternion.identity));
            
    //        particles[i].transform.SetParent(anchor.transform, false);
    //        particles[i].transform.localPosition = new Vector3(0, 0, 0);
    //    }
    //}

    //private void placeParticles()
    //{
    //    for (int i = 0; i < amount - 1; i++)
    //    {
    //        Vector3 newPosition = Quaternion.AngleAxis(Mathf.Acos(Random.Range(0f, 2 * Mathf.PI)), particles[i].transform.localPosition)
    //                                 * new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), 0f);

    //        particles[i].transform.localPosition = newPosition;
    //        //particles[i].transform.localPosition.Set(newPosition.x, newPosition.y, newPosition.z);
    //    }
    //}

    void OnCollisionEnter(Collision collision)
    {
        print(collision.collider);
    }

}
