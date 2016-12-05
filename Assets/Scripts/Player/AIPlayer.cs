using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Controller2D))]

public class AIPlayer : NetworkBehaviour {

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;
    public float jumpDeceleration = 0.5f;
    public float maxFallSpeed = -110f;

    private Gun gun;
    private SyncFlip syncFlip;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    private bool facingRight = false;
    private bool decelerating = false;
    private int jump;

    //private SpriteRenderer fire;
    AudioSource audio;
    private AnimationManager animator;

    //movement flags
    private Vector2 movementAxis;
    private bool buttonPressedJump;
    private bool buttonHeldJump;
    private bool buttonReleasedJump;
    private bool buttonPressedShoot;
    private bool buttonHeldShoot;

    Controller2D controller;
    NetworkManager networkManager;


    //Temp AI Spawning
    public GameObject AI;

    //For PathGeneration
    public Transform currentPlatform;

    void Awake() {
        syncFlip = GetComponent<SyncFlip>();
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
    }

    void Start() {

        //Debug.Log("is server: " + isServer);
        //Debug.Log("is local player: " + isLocalPlayer);
        //Debug.Log("is client: " + isClient);

        if (!isServer) {
            return;
        }

        controller = GetComponent<Controller2D>();
        audio = GetComponent<AudioSource>();
        gun = GetComponent<Gun>();
        animator = GetComponent<AnimationManager>();

        jump = 0;
        gravity = (2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        //syncPlayer.CmdSyncJetpack(false);

        movementAxis = new Vector2(0, 0);
        buttonPressedJump = false;
        buttonHeldJump = false;
        buttonReleasedJump = false;
        buttonPressedShoot = false;
        buttonHeldShoot = false;

        currentPlatform = null;
    }

    void Update() {

        if (!isServer) {
            return;
        }

        Vector2 input = movementAxis;
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        //flip sprite
        if (movementAxis.x > 0 && !facingRight) {
            flip();
        } else if (movementAxis.x < 0 && facingRight) {
            flip();
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
                    //syncPlayer.CmdSyncJetpack(true);
                }
            }

            if (jump == 0) {
                jump = 1;
            } else if (jump == 1) {
                jump = 2;
            }
        } else if (buttonReleasedJump) {
            decelerating = true;
            //syncPlayer.CmdSyncJetpack(false);
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

        //if (jump != 0 && controller.collisions.below) {
        //    Debug.Log("landing");
        //    animator.setIdle();
        //}

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
        if (buttonPressedShoot) {
            //gun.shoot();
        }
    }

    private void flip() {
        facingRight = !facingRight;
        syncFlip.CmdSyncFlip(facingRight);
    }

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
}
