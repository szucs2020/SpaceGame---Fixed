/*
 * Pistol.cs
 * Authors: Lorant
 * Description: This script controls the way
 *              the pistol works
 */
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Pistol : Gun{
	
    private GameObject laserDot;
	private Audio2D audio = Audio2D.singleton;

    void Start(){
        rpm = 220;
        bulletSpeed = 45;
        init();
        laserDot = Resources.Load("LaserDot") as GameObject;
    }


    [Command]
    public void CmdShoot(Vector2 direction, Vector2 position){

        GameObject laser = (GameObject)Instantiate(laserDot, position, Quaternion.identity);
        LaserDot dot = laser.GetComponent<LaserDot>();
        Player p = GetComponent<Player>();

        laser.GetComponent<Rigidbody2D>().velocity = bulletSpeed * direction;
        dot.bulletOwner = p;
        dot.gunType = p.gunNum;

        NetworkServer.Spawn(laser.gameObject);
    }

    public bool shoot(){

		bool shot = false;

        if (canShoot()){
            spawn.localEulerAngles = new Vector3(0f, spawn.localEulerAngles.y, 0f);

            Vector2 position = getSpawn().transform.position;
            Vector2 direction;
            Quaternion rotation = getSpawn().transform.rotation;

            if (player.isFacingRight()) {
                rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z);
                direction = rotation * Vector2.right;
            } else {
                rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z * -1);
                direction = rotation * Vector2.left;
            }

            //create bullet on all clients
            CmdShoot(direction, position);

            //set next shot time
            nextShot = Time.time + timeBetweenShots;
			shot = true;

            player.CmdPlaySound("Pistol");
        }
		return shot;
    }
}


