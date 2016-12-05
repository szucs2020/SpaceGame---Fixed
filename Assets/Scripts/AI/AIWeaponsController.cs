/*
 * AIWeaponsController.cs
 * Authors: Lajos Polya
 * Description: This script allows the AI to aim at its target and it also allows it to shoot in bursts
 */
using UnityEngine;
using System.Collections;

public class AIWeaponsController : MonoBehaviour {
	private Pistol pistol;
	private Player AI;
	private Transform Spawns;
	private Transform player;
	private SyncFlip AISync;
	private AIAimController AimController;
	private PlayerFinder playerFinder;
	//private BoxCollider2D playerCollider;

	private RaycastHit2D Hit;

	// Use this for initialization
	void Start () {
		pistol = transform.GetComponent<Pistol> ();
		AI = transform.GetComponent<Player> ();
		Spawns = transform.Find ("spawn");

		playerFinder = transform.GetComponent<PlayerFinder> ();
		player = playerFinder.getPlayer ().transform;
		AISync = transform.GetComponent<SyncFlip> ();
		//playerCollider = player.GetComponent<BoxCollider2D> ();

		AimController = transform.GetComponent<AIAimController> ();
	}
	
	// Update is called once per frame
	private float timePassed = 0f;
	private float randomTime = 0f;
	private bool burst = false;
	private int numberOfShots = 0;
	private int shotsFired = 0;
	private float burstTime = 0;
	void Update () {
		Hit = AimController.getHit ();

		timePassed += Time.deltaTime;
		if (Hit.transform != null && Hit.transform == player) {
			if (timePassed > 1f + randomTime) {
				burst = true;
				numberOfShots = (int)Random.Range (2.5f, 4.5f);
				randomTime = Random.Range (0f, 1f);
				timePassed = 0f;
			} else {
				AI.setbuttonPressedShoot (false);
			}
		}

		if (burst == true) {
			burstTime += Time.deltaTime;

			if (burstTime > 0.3f && shotsFired < numberOfShots) {
				shotsFired++;
				burstTime = 0;
			
				if (Hit.distance < 100) {
					AI.setbuttonPressedShoot (true);
					AI.setbuttonReleasedShoot (false);
				} else {
					AI.setbuttonPressedShoot (false);
					AI.setbuttonReleasedShoot (true);
				}
			} else {
				AI.setbuttonPressedShoot (false);

				if (shotsFired == numberOfShots) {
					shotsFired = 0;
					burst = false;
					burstTime = 0f;
					timePassed = 0f;
				}
			}
		}
	}
}
