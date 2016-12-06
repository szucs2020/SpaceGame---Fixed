/*
 * AIWeaponsController.cs
 * Authors: Lajos Polya
 * Description: This script allows the AI to aim at its target and it also allows it to shoot in bursts
 */
using UnityEngine;
using System.Collections;

public class AIWeaponsController : MonoBehaviour {
	private Player AI;
	private Transform player;
	private AIAimController AimController;
	//private PlayerFinder playerFinder;

	private RaycastHit2D Hit;

	// Use this for initialization
	void Start () {
		AI = transform.GetComponent<Player> ();

		/*playerFinder = transform.GetComponent<PlayerFinder> ();
		player = playerFinder.getPlayerTransform ();*/

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
		/*if (player == null) {
			player = playerFinder.getPlayerTransform ();
			return;
		}*/
		Hit = AimController.getHit ();

		timePassed += Time.deltaTime;
		if (Hit.transform != null && (Hit.transform.tag == "player" || Hit.transform.name == "LaserDot(Clone)")/*Hit.transform == player*/) {
			if (timePassed > randomTime) {
				burst = true;
				numberOfShots = (int)Random.Range (4.5f, 8.5f);
				randomTime = Random.Range (0.5f, 0.8f);
				timePassed = 0f;
			} else {
				AI.setbuttonPressedShoot (false);
			}
		}

		if (burst == true) {
			burstTime += Time.deltaTime;

			if (burstTime > 0.25f && shotsFired < numberOfShots) {
				shotsFired++;
				burstTime = 0;

				if (Hit.distance < 100) {
					AI.setbuttonPressedShoot (true);
					AI.setbuttonReleasedShoot (false);
				} else {
					AI.setbuttonPressedShoot (false);
					AI.setbuttonReleasedShoot (true);
				}
			} else if (burstTime <= 0.25f) {
				AI.setbuttonPressedShoot (false);
			} else if(shotsFired == numberOfShots) {
				AI.setbuttonPressedShoot (false);
				shotsFired = 0;
				burst = false;
				burstTime = 0f;
				timePassed = 0f;
			}
		}
	}
}
