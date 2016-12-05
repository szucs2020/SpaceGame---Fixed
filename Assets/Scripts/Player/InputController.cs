using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class InputController : NetworkBehaviour {

    private Player player;
	private bool released;
	private bool usingGamepad;

    void Start () {

		if (!isLocalPlayer) {
			return;
		}

        player = GetComponent<Player>();
		released = true;

		if (Input.GetJoystickNames().Length > 0){
			usingGamepad = true;
		} else {
			usingGamepad = false;
		}

    }

	void Update () {

		if (!isLocalPlayer) {
			return;
		}

        player.setMovementAxis(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        player.setbuttonPressedJump(Input.GetButtonDown("Jump"));
        player.setbuttonHeldJump(Input.GetButton("Jump"));
        player.setbuttonReleasedJump(Input.GetButtonUp("Jump"));

        //fix button pressed/held for the xbox controller
        if (usingGamepad){

            //gamepad right stick
            if (Input.GetAxisRaw("RightStickVertical") < -0.50) {
                player.setbuttonHeldAimUp(true);
            } else {
                player.setbuttonHeldAimUp(false);
            }

            if (Input.GetAxisRaw("RightStickVertical") > 0.50) {
                player.setbuttonHeldAimDown(true);
            } else {
                player.setbuttonHeldAimDown(false);
            }

            if (Input.GetAxisRaw("RightStickHorizontal") > 0.50) {
                player.setbuttonHeldAimRight(true);
            } else {
                player.setbuttonHeldAimRight(false);
            }

            if (Input.GetAxisRaw("RightStickHorizontal") < -0.50) {
                player.setbuttonHeldAimLeft(true);
            } else {
                player.setbuttonHeldAimLeft(false);
            }

            if (Input.GetAxis("Shoot") != 0) {
				player.setbuttonHeldShoot(true);
				if (released){
					player.setbuttonPressedShoot(true);
                    player.setbuttonReleasedShoot(false);
                    released = false;
				}

			} else {
				player.setbuttonHeldShoot(false);
				player.setbuttonPressedShoot(false);
                player.setbuttonReleasedShoot(true);
				released = true;
			}
		} else {

            player.setbuttonHeldAimLeft(Input.GetButton("AimLeft"));
            player.setbuttonHeldAimRight(Input.GetButton("AimRight"));
            player.setbuttonHeldAimUp(Input.GetButton("AimUp"));
            player.setbuttonHeldAimDown(Input.GetButton("AimDown"));

            player.setbuttonPressedShoot(Input.GetButtonDown("ShootButton"));
            player.setbuttonHeldShoot(Input.GetButton("ShootButton"));
            player.setbuttonReleasedShoot(Input.GetButtonUp("ShootButton"));
            player.setbuttonPressedAction(Input.GetButton("Action"));
        }
    }
}
