/*
 * Network.cs
 * Authors: Nigel
 * Description: Initializes and controls the animators of the head, torso, and legs of the Player object.
 */
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AnimationManager : NetworkBehaviour {
    public RuntimeAnimatorController blueHead;
    public RuntimeAnimatorController blueTorso;
    public RuntimeAnimatorController blueLegs;

    public RuntimeAnimatorController redHead;
    public RuntimeAnimatorController redTorso;
    public RuntimeAnimatorController redLegs;

    public RuntimeAnimatorController yellowHead;
    public RuntimeAnimatorController yellowTorso;
    public RuntimeAnimatorController yellowLegs;

    public RuntimeAnimatorController greenHead;
    public RuntimeAnimatorController greenTorso;
    public RuntimeAnimatorController greenLegs;

    Transform head;
    Transform torso;
    Transform legs;
    Player player;
    LobbyPlayer myPlayer;

    Animator headAnimator;
    Animator torsoAnimator;
    Animator legsAnimator;

	// Use this for initialization

    void Awake() {
        head = transform.Find("Head");
        torso = transform.Find("Torso");
        legs = transform.Find("Legs");

        headAnimator = head.GetComponent<Animator>();
        torsoAnimator = torso.GetComponent<Animator>();
        legsAnimator = legs.GetComponent<Animator>();

        player = GetComponent<Player>();
    }

	void Start () {

        //Search for the LobbyPlayer associated with this Player.
        LobbyPlayer[] lobbyPlayers = Object.FindObjectsOfType<LobbyPlayer>();
        foreach (LobbyPlayer lobbyPlayer in lobbyPlayers) {
            if (lobbyPlayer.slot == player.playerSlot) {
                myPlayer = lobbyPlayer;
            }
        }

        if (myPlayer != null) {
            switch (myPlayer.GetTeam()) {
                case 0:
                    headAnimator.runtimeAnimatorController = blueHead as RuntimeAnimatorController;
                    torsoAnimator.runtimeAnimatorController = blueTorso as RuntimeAnimatorController; 
                    legsAnimator.runtimeAnimatorController = blueLegs as RuntimeAnimatorController;
                    break;
                case 1:
                    headAnimator.runtimeAnimatorController = redHead as RuntimeAnimatorController;
                    torsoAnimator.runtimeAnimatorController = redTorso as RuntimeAnimatorController;
                    legsAnimator.runtimeAnimatorController = redLegs as RuntimeAnimatorController;
                    break;
                case 2:
                    headAnimator.runtimeAnimatorController = yellowHead as RuntimeAnimatorController;
                    torsoAnimator.runtimeAnimatorController = yellowTorso as RuntimeAnimatorController;
                    legsAnimator.runtimeAnimatorController = yellowLegs as RuntimeAnimatorController;
                    break;
                case 3:
                    headAnimator.runtimeAnimatorController = greenHead as RuntimeAnimatorController;
                    torsoAnimator.runtimeAnimatorController = greenTorso as RuntimeAnimatorController;
                    legsAnimator.runtimeAnimatorController = greenLegs as RuntimeAnimatorController;
                    break;
            }
        }

    }

    //upper body
    public void setNeutral() {
        headAnimator.SetTrigger("Neutral");
        torsoAnimator.SetTrigger("Neutral");
        player.setCurrentPosition(2);
        CmdsetNeutral();
    }

    [Command]
    public void CmdsetNeutral() {
        headAnimator.SetTrigger("Neutral");
        torsoAnimator.SetTrigger("Neutral");
        player.setCurrentPosition(2);
        RpcsetNeutral();
    }

    [ClientRpc]
    public void RpcsetNeutral() {
        headAnimator.SetTrigger("Neutral");
        torsoAnimator.SetTrigger("Neutral");
        player.setCurrentPosition(2);
    }

    public void setUp() {
        headAnimator.SetTrigger("Up");
        torsoAnimator.SetTrigger("Up");
        player.setCurrentPosition(4);
        CmdsetUp();
    }

    [Command]
    public void CmdsetUp() {
        headAnimator.SetTrigger("Up");
        torsoAnimator.SetTrigger("Up");
        player.setCurrentPosition(4);
        RpcsetUp();
    }

    [ClientRpc]
    public void RpcsetUp() {
        headAnimator.SetTrigger("Up");
        torsoAnimator.SetTrigger("Up");
        player.setCurrentPosition(4);
    }

    public void setUpTilt() {
        headAnimator.SetTrigger("Up Tilt");
        torsoAnimator.SetTrigger("Up Tilt");
        player.setCurrentPosition(3);
        CmdsetUpTilt();
    }

    [Command]
    public void CmdsetUpTilt() {
        headAnimator.SetTrigger("Up Tilt");
        torsoAnimator.SetTrigger("Up Tilt");
        player.setCurrentPosition(3);
        RpcsetUpTilt();
    }

    [ClientRpc]
    public void RpcsetUpTilt() {
        headAnimator.SetTrigger("Up Tilt");
        torsoAnimator.SetTrigger("Up Tilt");
        player.setCurrentPosition(3);
    }

    public void setDown() {
        headAnimator.SetTrigger("Down");
        torsoAnimator.SetTrigger("Down");
        player.setCurrentPosition(0);
        CmdsetDown();
    }

    [Command]
    public void CmdsetDown() {
        headAnimator.SetTrigger("Down");
        torsoAnimator.SetTrigger("Down");
        player.setCurrentPosition(0);
        RpcsetDown();
    }

    [ClientRpc]
    public void RpcsetDown() {
        headAnimator.SetTrigger("Down");
        torsoAnimator.SetTrigger("Down");
        player.setCurrentPosition(0);
    }

    public void setDownTilt() {
        headAnimator.SetTrigger("Down Tilt");
        torsoAnimator.SetTrigger("Down Tilt");
        player.setCurrentPosition(1);
        CmdsetDownTilt();
    }

    [Command]
    public void CmdsetDownTilt() {
        headAnimator.SetTrigger("Down Tilt");
        torsoAnimator.SetTrigger("Down Tilt");
        player.setCurrentPosition(1);
        RpcsetDownTilt();
    }

    [ClientRpc]
    public void RpcsetDownTilt() {
        headAnimator.SetTrigger("Down Tilt");
        torsoAnimator.SetTrigger("Down Tilt");
        player.setCurrentPosition(1);
    }

    public void setPistol() {
        torsoAnimator.SetTrigger("Pistol");
        CmdsetPistol();
    }

    [Command]
    public void CmdsetPistol() {
        torsoAnimator.SetTrigger("Pistol");
        RpcsetPistol();
    }

    [ClientRpc]
    public void RpcsetPistol() {
        torsoAnimator.SetTrigger("Pistol");
    }

    public void setShotgun() {
        torsoAnimator.SetTrigger("Shotgun");
        CmdsetShotgun();
    }

    [Command]
    public void CmdsetShotgun() {
        torsoAnimator.SetTrigger("Shotgun");
        RpcsetShotgun();
    }

    [ClientRpc]
    public void RpcsetShotgun() {
        torsoAnimator.SetTrigger("Shotgun");
    }

    public void setCannon() {
        torsoAnimator.SetTrigger("Cannon");
        CmdsetCannon();
    }

    [Command]
    public void CmdsetCannon() {
        torsoAnimator.SetTrigger("Cannon");
        RpcsetCannon();
    }

    [ClientRpc]
    public void RpcsetCannon() {
        torsoAnimator.SetTrigger("Cannon");
    }

    //legs
    public void setIdle() {
        legsAnimator.SetTrigger("Idle");
        CmdsetIdle();
    }

    [Command]
    public void CmdsetIdle() {
        legsAnimator.SetTrigger("Idle");
        RpcsetIdle();
    }

    [ClientRpc]
    public void RpcsetIdle() {
        legsAnimator.SetTrigger("Idle");
    }

    public void setWalkForward() {
        legsAnimator.SetTrigger("Walk Forward");
        CmdsetWalkForward();
    }

    [Command]
    public void CmdsetWalkForward() {
        legsAnimator.SetTrigger("Walk Forward");
        RpcsetWalkForward();
    }

    [ClientRpc]
    public void RpcsetWalkForward() {
        legsAnimator.SetTrigger("Walk Forward");
    }

    public void setWalkBackward() {
        legsAnimator.SetTrigger("Walk Backward");
        CmdsetWalkBackward();
    }

    [Command]
    public void CmdsetWalkBackward() {
        legsAnimator.SetTrigger("Walk Backward");
        RpcsetWalkBackward();
    }

    [ClientRpc]
    public void RpcsetWalkBackward() {
        legsAnimator.SetTrigger("Walk Backward");
    }

    //jumping
    public void setJump() {
        legsAnimator.SetTrigger("Jump");
        CmdsetJump();
    }

    [Command]
    public void CmdsetJump() {
        legsAnimator.SetTrigger("Jump");
        RpcsetJump();
    }

    [ClientRpc]
    public void RpcsetJump() {
        legsAnimator.SetTrigger("Jump");
    }

    public void setFall() {
        legsAnimator.SetTrigger("Fall");
        CmdsetFall();
    }

    [Command]
    public void CmdsetFall() {
        legsAnimator.SetTrigger("Fall");
        RpcsetFall();
    }

    [ClientRpc]
    public void RpcsetFall() {
        legsAnimator.SetTrigger("Fall");
    }
}
