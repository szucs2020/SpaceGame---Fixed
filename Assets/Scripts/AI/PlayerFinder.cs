using UnityEngine;
using System.Collections;

public class PlayerFinder : MonoBehaviour {

	private GameObject player;

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
