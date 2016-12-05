/*
 * SyncFlip.cs
 * Authors: Christian
 * Description: This class synchronizes the player direction over the network. 
 */
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SyncFlip : NetworkBehaviour {

    //local player for callbacks
    public Player player;
    private RectTransform health;
	private RectTransform name;

    //flip
	[SyncVar(hook="FlipHook")]
	private bool facingRight = false;

    void Start() {
        health = transform.FindChild("HealthCanvas").GetComponent<RectTransform>();
		name = transform.FindChild("NameCanvas").GetComponent<RectTransform>();
    }

    [Command]
	public void CmdSyncFlip(bool direction){

        float scale;
		facingRight = direction;
		Vector3 temp = transform.localScale;

		if (facingRight){
			temp.x = -1;
            scale = -1;
        } else{
			temp.x = 1;
            scale = 1;
        }
        transform.localScale = temp;
        health.localScale = new Vector3(scale, health.localScale.y, health.localScale.z);
		name.localScale = new Vector3(scale, name.localScale.y, name.localScale.z);
    }

	void FlipHook(bool direction){

        float scale;
        facingRight = direction;
		Vector3 temp = transform.localScale;

		if (facingRight){
			temp.x = -1;
            scale = -1;
        } else {
			temp.x = 1;
            scale = 1;
        }
		transform.localScale = temp;
        health.localScale = new Vector3(scale, health.localScale.y, health.localScale.z);
		name.localScale = new Vector3(scale, name.localScale.y, name.localScale.z);
    }

	public bool getFacingRight() {
		return facingRight;
	}
}