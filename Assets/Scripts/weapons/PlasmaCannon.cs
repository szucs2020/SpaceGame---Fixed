/*
 * PlasmaCannon.cs
 * Authors: Lorant
 * Description: This script controls the way the
 *              plasma cannon works
 */
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlasmaCannon : Gun{
    private GameObject plasmaBallPrefab;
	private Audio2D audio = Audio2D.singleton;

    void Start(){
        rpm = 450;
        bulletSpeed = 45;

        clipSize = 2;
        shots = 0;
        reloadTime = 3;

        init();
        plasmaBallPrefab = Resources.Load("PlasmaBall") as GameObject;
    }


    [Command]
    public void CmdShoot(Vector2 direction, Vector2 position){
        GameObject plasmaBall;
        plasmaBall = (GameObject)Instantiate(plasmaBallPrefab, position, Quaternion.identity);
        plasmaBall.GetComponent<PlasmaBall>().bulletOwner = GetComponent<Player>();
        plasmaBall.GetComponent<Rigidbody2D>().velocity = bulletSpeed * direction;

        NetworkServer.Spawn(plasmaBall);
    }

    void Update() {
        if (reloading) {
            if (Time.time >= endReloadTime) {
                reloading = false;
            }
        }
    }

    public bool shoot(){

		bool shot = false;

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
		}

        if (shots == clipSize){
            reload();
        }
		return shot;
    }
}


