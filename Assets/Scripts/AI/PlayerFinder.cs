/*
 * PlayerFinder.cs
 * Authors: Lajos Polya
 * Description: This script has a copy of the AIs target which it distributed to AIController, AIAimCOntroller, and AIWeaponsController.
 */
using UnityEngine;
using System.Collections;

public class PlayerFinder : MonoBehaviour {

	private GameObject player;

	private AIController controller;
	private AIAimController aimController;

	void Start() {
		controller = transform.GetComponent<AIController> ();
		aimController = transform.GetComponent<AIAimController> ();
	}


	public GameObject getPlayer() {
		findPlayer ();

		return player;
	}

	public Transform getPlayerTransform() {
		findPlayer ();

		if (player != null) {
			return player.transform;
		}
		return null;

	}

	public void resetPlayer() {
		player = null;
		findPlayer ();

		/*These methods set their player var to null
		 * I am not explicitly setting theit player var to a value
		 * because this way the scripts will find that the player is null
		 * and then set the new player var along with the playerComponent var
		 */
		controller.resetPlayer ();
		aimController.resetPlayer ();

	}

	//if findNewPlayer is True, set the player for all of the classes
	private void findPlayer() {
		if (player == null) {
			GameObject[] players = GameObject.FindGameObjectsWithTag ("player");

			int i = int.MaxValue;
			float minDist = float.MaxValue;
			int index = 0;
			for (i = 0; i < players.Length; i++) {
				if (players [i] != gameObject) {
					float curDist = (transform.position - players [i].transform.position).sqrMagnitude;
					if (curDist < minDist) {
						minDist = curDist;
						index = i;
					}
				}
			}
			if (i != int.MaxValue) {
				player = players [index];
			}
		}
	}
}
