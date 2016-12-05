/*
 * Shotgun.cs
 * Authors: Lorant
 * Description: This script controls how the shotgun works
 */
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Shotgun : Gun {
	
    private GameObject laserDot;
	private Audio2D audio = Audio2D.singleton;

    void Start(){
        rpm = 450;
        bulletSpeed = 45;

        clipSize = 6;
        shots = 0;
        reloadTime = 2;

        init();
        laserDot = Resources.Load("LaserDot") as GameObject;
    }

    public GameObject[] generateShotgunShells(Vector2 direction, Vector2 position){

        GameObject[] lasers = new GameObject[5];

        for (int i = 0; i < 5; i++){
            lasers[i] = (GameObject)Instantiate(laserDot, position, Quaternion.identity);
        }
        return lasers;
    }


    [Command]
    public void CmdShoot(Vector2 direction, Vector2 position){
        GameObject[] lasers;
        lasers = generateShotgunShells(direction, position);

        for (int i = 0; i < lasers.Length; i++){
            if (i % 2 == 0){
                lasers[i].GetComponent<Rigidbody2D>().velocity = bulletSpeed * (Quaternion.AngleAxis(i * 1.5f, Vector3.back) * direction);

            } else {
                lasers[i].GetComponent<Rigidbody2D>().velocity = bulletSpeed * (Quaternion.AngleAxis(i * -1.5f, Vector3.back) * direction);
            }
            lasers[i].GetComponent<LaserDot>().bulletOwner = GetComponent<Player>();
            NetworkServer.Spawn(lasers[i].gameObject);
        }
    }
	
    public bool shoot(){

		bool shot = false;

        if (reloading)
        {
            if (Time.time >= endReloadTime)
            {
                reloading = false;
            }
        }

        if (canShoot()){

            spawn.localEulerAngles = new Vector3(0f, spawn.localEulerAngles.y, 0f);

            Vector2 position = getSpawn().transform.position;
            Vector2 direction;
            Quaternion rotation = getSpawn().transform.rotation;

            if (player.isFacingRight()){
                rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z * -1);
                direction = rotation * Vector2.right;
            } else {
                rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z * -1);
                direction = rotation * Vector2.left;
            }

            //create bullet on all clients
            CmdShoot(direction, position);

            //set next shot time
            nextShot = Time.time + timeBetweenShots;
            shots++;
			shot = true;

			audio.PlaySound("Shotgun");
        }

        if (shots == clipSize)
        {
            reload();
        }
		return shot;
    }
}


