/*
 * Network.cs
 * Authors: Christian. Animation related code was done with Nigel. 
 * Description: This is the main player controller class. 
 */
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]

public class Player : NetworkBehaviour {

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;
    public float jumpDeceleration = 0.5f;
    public float maxFallSpeed = -110f;

    private Pistol pistol;
    private Shotgun shotgun;
    private PlasmaCannon plasmaCannon;
    private SyncFlip syncFlip;

    private bool charging = false;
    private bool charged = false;
    private bool cannonFinished = true;
    private bool shootReleased = true;
    [SyncVar(hook = "ChangeWeapon")]
    public int gunNum = 1;
    //  1  Pistol
    //  2  Shotgun
    //  3  PlasmaCannon

    Pickup pickup;
    [SyncVar]
    private int pickupId;
    private bool nearPickup = false;
    private bool pickedUp = false;
    private GameObject dropShotgun;
    private GameObject dropCannon;

    //movement variables
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    private bool facingRight = false;
    private bool decelerating = false;
    private int jump;
    private int currentPosition;

    //private SpriteRenderer fire;
    private AnimationManager animator;

    //movement flags
    private Vector2 movementAxis;

    private bool buttonPressedJump;
    private bool buttonHeldJump;
    private bool buttonReleasedJump;

    private bool buttonPressedShoot;
    private bool buttonHeldShoot;
    private bool buttonReleasedShoot;

    private bool buttonPressedAction;
    private bool buttonPressedReload;

    private bool buttonHeldAimLeft;
    private bool buttonHeldAimRight;
    private bool buttonHeldAimUp;
    private bool buttonHeldAimDown;

    Controller2D controller;
    NetworkManager networkManager;

    [SyncVar]
    public int playerSlot;

    [SyncVar(hook = "playerNameChanged")]
    public string playerName;

    //Temp AI Spawning
    public GameObject AI;

    //For PathGeneration
    public Transform currentPlatform;
    private bool isAI = false;

    public Audio2D audio2D;
    private Chat chat;

    void Awake() {
        audio2D = Audio2D.singleton;
        chat = Chat.singleton;
    }

    void Start() {

        syncFlip = GetComponent<SyncFlip>();
        syncFlip.player = this;
        StartCoroutine("nameFix");
        animator = GetComponent<AnimationManager>();

        CameraExpander cam = GameObject.Find("Main Camera").GetComponent<CameraExpander>();
        cam.UpdatePlayers();

        if (!isLocalPlayer && !isAI) {
            return;
        }
        controller = GetComponent<Controller2D>();

        pistol = GetComponent<Pistol>();
        shotgun = GetComponent<Shotgun>();
        plasmaCannon = GetComponent<PlasmaCannon>();

        dropShotgun = Resources.Load("DropShotgun") as GameObject;
        dropCannon = Resources.Load("DropCannon") as GameObject;

        jump = 0;
        gravity = (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        movementAxis = new Vector2(0, 0);
        buttonPressedJump = false;
        buttonHeldJump = false;
        buttonReleasedJump = false;
        buttonPressedShoot = false;
        buttonHeldShoot = false;
        buttonReleasedShoot = true;
        buttonPressedAction = false;
        buttonPressedReload = false;

        currentPlatform = null;
        currentPosition = 2;
    }

    void Update() {

        if (!isLocalPlayer && !isAI) {
            return;
        }

        Vector2 input = movementAxis;
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        if (buttonHeldAimLeft) {
            if (facingRight) {
                flip();
            }
        } else if (buttonHeldAimRight) {
            if (!facingRight) {
                flip();
            }
        }

        //Change aiming angle based on which keys are pressed.
        if (buttonHeldAimUp && buttonHeldAimLeft || buttonHeldAimUp && buttonHeldAimRight) {
            animator.setUpTilt();
        } else if (buttonHeldAimDown && buttonHeldAimLeft || buttonHeldAimDown && buttonHeldAimRight) {
            animator.setDownTilt();
        } else if (buttonHeldAimUp) {
            animator.setUp();
        } else if (buttonHeldAimDown) {
            animator.setDown();
        } else if (buttonHeldAimLeft || buttonHeldAimRight) {
            animator.setNeutral();
        } else {
            animator.setNeutral();
        }

        //switcing weapons
        if (Input.GetKeyDown("1")) {
            CmdChangeWeapon(1);
        } else if (Input.GetKeyDown("2")) {
            CmdChangeWeapon(2);
        } else if (Input.GetKeyDown("3")) {
            CmdChangeWeapon(3);
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        //walking
        if (controller.collisions.below) {
            currentPlatform = controller.collisions.platform;
            if (targetVelocityX < 0) {
                if (facingRight) {
                    animator.setWalkBackward();
                } else {
                    animator.setWalkForward();
                }
            } else if (targetVelocityX > 0) {
                if (!facingRight) {
                    animator.setWalkBackward();
                } else {
                    animator.setWalkForward();
                }
            } else {
                animator.setIdle();
            }
        }

        //jumping
        if (buttonPressedJump) {
            if (controller.collisions.below || jump == 1) {
                velocity.y = maxJumpVelocity;
                decelerating = false;
                if (jump == 1) {
                    CmdPlaySound("Boost");
                }
            }

            if (jump == 0) {
                jump = 1;
                CmdPlaySound("Jump");
            } else if (jump == 1) {
                jump = 2;
            }
        } else if (buttonReleasedJump) {
            decelerating = true;
        }

        if (!controller.collisions.below && jump == 0) {
            jump = 1;
        }

        if (velocity.y < 0) {
            animator.setFall();
            //syncPlayer.CmdSyncJetpack(false);
        } else if (velocity.y > 0) {
            animator.setJump();
        }

        //decelerate after jump
        if (decelerating && velocity.y > minJumpVelocity) {
            if (velocity.y - jumpDeceleration > minJumpVelocity) {
                velocity.y -= jumpDeceleration;
            } else {
                velocity.y = minJumpVelocity;
            }
        }

        //gravity, with max fall speed
        if (velocity.y > maxFallSpeed) {
            if (velocity.y - (gravity * Time.deltaTime) > maxFallSpeed) {
                velocity.y -= gravity * Time.deltaTime;
            } else {
                velocity.y = maxFallSpeed;
            }
        }

        controller.Move(velocity * Time.deltaTime, input);

        if (controller.collisions.below) {
            velocity.y = 0;
            decelerating = false;
            jump = 0;
        } else {
            if (controller.collisions.above) {
                velocity.y = 0;
                decelerating = false;
            }
        }

        //shooting
        if (buttonReleasedShoot) {
            shootReleased = true;
            charged = false;
            charging = false;
            if (cannonFinished) {
                CmdStopSound("Plasma");
            }

            StopCoroutine("chargeCannon");
        } else if (buttonHeldShoot && gunNum == 3 && cannonFinished && shootReleased && plasmaCannon.canShoot()) {

            StartCoroutine("chargeCannon");
            if (!charging) {
                charging = true;
                CmdPlaySound("Plasma");
            }

            if (charged && shootReleased) {

                if (gunNum == 1) {
                    pistol.shoot();
                } else if (gunNum == 2) {
                    shotgun.shoot();
                } else if (gunNum == 3) {
                    plasmaCannon.shoot();
                    charging = false;
                    cannonFinished = false;
                    StartCoroutine("WaitForCannonDecay");
                }
                shootReleased = false;
            }
        } else if (buttonPressedShoot && gunNum != 3) {
            if (gunNum == 1) {
                pistol.shoot();
            } else if (gunNum == 2) {
                shotgun.shoot();
            }
        }

        if (buttonPressedReload) {
            if (gunNum == 2) {
                shotgun.reload();
            } else if (gunNum == 3) {
                plasmaCannon.reload();
            }
        }

        if (buttonPressedAction)
        {
            CmdDropWeapon(gunNum);

            if (nearPickup)
            {
                audio2D.PlaySound("Reload");
                CmdChangeToPickup();
                pickup.destroy();
                nearPickup = false;
            }
            else if (!nearPickup)
            {
                CmdChangeWeapon(1);
            }
        }

        //if (chat.chatInput != null)
        //{
        //    string message = chat.chatInput.text;


        //    if (!string.IsNullOrEmpty(message.Trim()) && Input.GetKeyDown("return"))
        //    {
        //        message = playerName + ": " + message + "\n";
        //        CmdPrintMessage(message);
        //        chat.chatInput.text = "";
        //    }
        //}

    }

    public void Die() {
        CmdPlaySound("Die");
        Destroy(gameObject);
    }

    IEnumerator chargeCannon() {
        yield return new WaitForSeconds(1.4f);
        charged = true;
    }

    IEnumerator WaitForCannonDecay() {
        yield return new WaitForSeconds(1f);
        cannonFinished = true;
    }

    IEnumerator nameFix() {
        yield return new WaitForSeconds(1.0f);
        playerNameChanged(playerName);
    }

    private void playerNameChanged(string pn) {
        Text Name = transform.FindChild("NameCanvas").FindChild("Name").GetComponent<Text>();
        Name.text = pn;
    }

    //flip 2D sprite
    private void flip() {
        facingRight = !facingRight;
        syncFlip.CmdSyncFlip(facingRight);
    }

    private void RemoveGun() {
        Destroy(GetComponent<Gun>());
    }

    //getters & setters
    public bool isFacingRight() {
        return facingRight;
    }
    public void setMovementAxis(Vector2 input) {
        this.movementAxis = input;
    }
    public void setbuttonPressedJump(bool input) {
        this.buttonPressedJump = input;
    }
    public void setbuttonHeldJump(bool input) {
        this.buttonHeldJump = input;
    }
    public void setbuttonReleasedJump(bool input) {
        this.buttonReleasedJump = input;
    }
    public void setbuttonPressedShoot(bool input) {
        this.buttonPressedShoot = input;
    }
    public void setbuttonHeldShoot(bool input) {
        this.buttonHeldShoot = input;
    }
    public void setbuttonReleasedShoot(bool input) {
        this.buttonReleasedShoot = input;
    }
    public void setbuttonPressedAction(bool input) {
        this.buttonPressedAction = input;
    }

    public void setbuttonPressedReload(bool input) {
        this.buttonPressedReload = input;
    }

    //right stick / right hand aiming
    public void setbuttonHeldAimLeft(bool input) {
        this.buttonHeldAimLeft = input;
    }
    public void setbuttonHeldAimRight(bool input) {
        this.buttonHeldAimRight = input;
    }
    public void setbuttonHeldAimUp(bool input) {
        this.buttonHeldAimUp = input;
    }
    public void setbuttonHeldAimDown(bool input) {
        this.buttonHeldAimDown = input;
    }

    //current position
    public void setCurrentPosition(int currentPosition) {
        this.currentPosition = currentPosition;
    }
    public int getCurrentPosition() {
        return this.currentPosition;
    }

    public void setIsAI(bool isAI) {
        this.isAI = isAI;
    }

    public bool getIsAI() {
        return this.isAI;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Pickup")
        {
            nearPickup = true;
            pickup = col.gameObject.GetComponent<Pickup>();
        }

    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.tag == "Pickup")
        {
            nearPickup = false;
        }
    }

    //audio RPCs/Commands
    [Command]
    public void CmdPlaySound(string name) {
        RpcPlaySound(audio2D.GetSoundIndex(name));
    }

    [ClientRpc]
    void RpcPlaySound(int name) {
        audio2D.PlaySound(name);
    }

    [Command]
    public void CmdStopSound(string name) {
        RpcStopSound(audio2D.GetSoundIndex(name));
    }

    [ClientRpc]
    void RpcStopSound(int name) {
        audio2D.StopSound(name);
    }

    [Command]
    private void CmdChangeToPickup() {
        pickupId = pickup.id;
        CmdChangeWeapon(pickupId);
    }

    [Command]
    public void CmdChangeWeapon(int weaponNum) {
        gunNum = weaponNum;

        if (weaponNum == 1) {
            animator.setPistol();
        } else if (weaponNum == 2) {
            animator.setShotgun();
        } else if (weaponNum == 3) {
            animator.setCannon();
        }
    }

    [Command]
    private void CmdDropWeapon(int weaponNum)
    {
        GameObject droppedGun = null;

        if (weaponNum == 1)
        {
            return;
        }
        else if (weaponNum == 2)
        {
            droppedGun = (GameObject)Instantiate(dropShotgun, this.transform.position, Quaternion.identity);
        }
        else if (weaponNum == 3)
        {
            droppedGun = (GameObject)Instantiate(dropCannon, this.transform.position, Quaternion.identity);
        }

        Rigidbody2D rb = droppedGun.GetComponent<Rigidbody2D>();
        //rb.AddForce(Vector2.up * 12, ForceMode2D.Impulse);
        rb.velocity = Vector2.up * 12;
        rb.AddTorque(-500f);
        NetworkServer.Spawn(droppedGun);
        Destroy(droppedGun, 3f);
    }

    void ChangeWeapon(int weaponNum) {
        gunNum = weaponNum;
    }

    [Command]
    void CmdPrintMessage(string message)
    {
        RpcPrintMessage(message);
    }

    [ClientRpc]
    void RpcPrintMessage(string message)
    {
        chat.PrintMessage(message);
    }
}
