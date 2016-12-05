/*
 * AIController.cs
 * Authors: Lajos Polya
 * Description: This script cotrolls how the AI moves around the map including jumping
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : MonoBehaviour {
	private AStar pathFinder;
	private List<Node> path;
	public Node target;
	private Player AI;
	private GameObject player;
	private Player playerComponent;
	private Controller2D controller;
	private PlayerFinder playerFinder;

	private float playerHeight = 0f;
	private float AIHeight = 0f;
	private bool hasPath = false;

	private Platform savedPlatform = null;
	private bool jumpingToNextPlatform = false;

	// Movement State
	private States state;

	//Same Platform Movement
	private bool inMotion;
	private Vector3 moveTo;
	private bool mightJump;

	// Use this for initialization
	void Start () {
		pathFinder = this.GetComponent<AStar> ();
		controller = this.GetComponent<Controller2D> ();

		AI = transform.GetComponent<Player> ();
		AI.setIsAI (true);

		playerFinder = transform.GetComponent<PlayerFinder> ();
		player = playerFinder.getPlayer ().gameObject;

		//Movement
		playerComponent = player.GetComponent<Player> ();
		playerHeight = player.GetComponent<BoxCollider2D> ().bounds.size.y;
		AIHeight = transform.GetComponent<BoxCollider2D> ().bounds.size.y;

		path = pathFinder.FindShortestPath (playerComponent);
		target = path [0];
		path.RemoveAt (0);

		// Movement State
		state = States.Follow;

		//Same Platform Movement
		inMotion = false;
		mightJump = true;
	}

	private double timedelta = 0;
	void Update () {
		if (player == null) {
			AI.setMovementAxis (new Vector2 (0, 0));

			player = playerFinder.getPlayer ();

			if (player != null) {
				playerComponent = player.GetComponent<Player> ();
				playerHeight = player.GetComponent<BoxCollider2D> ().bounds.size.y;
			}
			return;
		}
        timedelta += Time.deltaTime;
		if (timedelta < 2.5f) {
			return;
		}

		if (savedPlatform == null) {
			savedPlatform = AI.currentPlatform.GetComponent<Platform> ();
		} else if (savedPlatform.transform != AI.currentPlatform) {
			savedPlatform = AI.currentPlatform.GetComponent<Platform> ();
			jumpingToNextPlatform = false;
			hasPath = false;
		}

		if (state == States.SamePlatform) {
			print ("SAMEPLATFORM");

			if (AI.currentPlatform != playerComponent.currentPlatform) {
				state = States.Follow;
			}

			path.Clear ();
			hasPath = false;
			target = null;

			AI.setbuttonPressedJump (false);
			AI.setbuttonReleasedJump (false);

			if (inMotion == false) {
				int index = (int)Random.Range (0, 4.99f);

				if (index < 2) {
					moveTo = AI.currentPlatform.GetChild (index).position;
				} else {
					moveTo = new Vector3 (player.transform.position.x + Random.Range (-30f, 30f), player.transform.position.y, 0f);
					mightJump = true;

				}
				inMotion = true;
			} else {
				
				if (mightJump == true && (savedPlatform.getRight () - transform.position.x < 15f || transform.position.x - savedPlatform.getLeft () < 15f)) {
					if (moveTo.x > savedPlatform.getRight () || moveTo.x < savedPlatform.getLeft ()) {
						Move (moveTo, true);
					}
				} else {
					Move (moveTo, false);
				}

				if (Mathf.Abs (transform.position.x - moveTo.x) < 2f) {
					inMotion = false;
					mightJump = false;
				}
			}

			/*float variablePos = Random.Range(-5f, 5f);

			if (transform.position.x - player.transform.position.x < 40f || jumpingToNextPlatform == true) {
				if ((player.transform.position.x < transform.position.x && transform.position.x - player.transform.position.x < 50f) || jumpingToNextPlatform == true) {
					if (jumpingToNextPlatform == false) {
						Move (player.transform.position + new Vector3 (variablePos + 40f, -playerHeight + 3, 0), false); //Add a random x value so it doesn't always stay the same distance
					}

					if (savedPlatform.getRight () - transform.position.x < 15f) {
						jumpingToNextPlatform = true;
						Transform targetNode = findNearestPlatform (playerComponent.currentPlatform.GetComponent<Platform> (), true);
						Move (targetNode.position, true);
					}
				} else if (player.transform.position.x > transform.position.x && player.transform.position.x - transform.position.x < 50f || jumpingToNextPlatform == true) {
					if (jumpingToNextPlatform == false) {
						Move (player.transform.position - new Vector3 (variablePos + 40f, -playerHeight + 3, 0), false); //Add a random x value so it doesn't always stay the same distance
					}

					if (transform.position.x - savedPlatform.getLeft () < 15f) {
						jumpingToNextPlatform = true;
						Transform targetNode = findNearestPlatform (playerComponent.currentPlatform.GetComponent<Platform> (), false);
						Move (targetNode.position, true);
					} 
				}
			} else {
				Move (player.transform.position + new Vector3 (variablePos + 40f, -playerHeight + 3, 0), false); //Add a random x value so it doesn't always stay the same distance
			}*/

		} else if (state == States.Follow) {
			print ("FOLLOW");
			bool onNeighbourPlatform = false;

			if (AI.currentPlatform == playerComponent.currentPlatform) {
				state = States.SamePlatform;
			}

			foreach (Transform neighbourPlatform in AI.currentPlatform.GetComponent<Platform>().neighbours) {
				if (neighbourPlatform == playerComponent.currentPlatform) {
					onNeighbourPlatform = true;
					break;
				}
			}

			//On a neighbouring platform to the Player
			if (onNeighbourPlatform == true) {
				path.Clear ();
				hasPath = false;
				target = null;
				AI.setbuttonPressedJump (false);
				AI.setbuttonReleasedJump (false);

				float variablePos = Random.Range(-5f, 5f);

				if (player.transform.position.x < transform.position.x && transform.position.x - player.transform.position.x < 30f) { //If within a certain distance stop
					AI.setMovementAxis (new Vector2 (0, 0));
				} else if (player.transform.position.x > transform.position.x && player.transform.position.x - transform.position.x < 30f) { //If within a certain distance stop
					AI.setMovementAxis (new Vector2 (0, 0));
				} else if (player.transform.position.x < transform.position.x && transform.position.x - player.transform.position.x > 30f) { //If not within a certain distance continue
					//nodes[1] represents the second(last) node on platform
					if (transform.position.x - savedPlatform.getLeft () < 15f) {
						Move (playerComponent.currentPlatform.GetComponent<Platform> ().nodes [1].transform.position, true);
					} else {
						Move (playerComponent.currentPlatform.GetComponent<Platform> ().nodes [1].transform.position, false);
					}
				} else if (player.transform.position.x > transform.position.x && player.transform.position.x - transform.position.x > 30f) { //If not within a certain distance continue
					//nodes[0] represents the first node on platform
					if (savedPlatform.getRight () - transform.position.x < 15f) {
						Move(playerComponent.currentPlatform.GetComponent<Platform> ().nodes[0].transform.position, true);
					} else {
						Move(playerComponent.currentPlatform.GetComponent<Platform> ().nodes[0].transform.position, false);
					}
				}
			} else { //target represents a node on the platform
				if (!hasPath) {
					path = pathFinder.FindShortestPath (playerComponent);
					target = path [0];
					path.RemoveAt (0);
					hasPath = true;
				}

				if (target != null && target.transform.parent == AI.currentPlatform) {
					AI.setbuttonPressedJump (false);
					AI.setbuttonReleasedJump (false);
					WalkOnPlatform ();
				} else if (target != null && target.transform.parent != AI.currentPlatform) {
					WalkOnPlatform();
				} else {
					AI.setMovementAxis (new Vector2 (0, 0));
				}
			}

		} else if (state == States.Disregard) {
			print ("DISREGARD");
		}

		/*if (AI.currentPlatform == playerComponent.currentPlatform) {
			path.Clear ();
			hasPath = false;
			target = null;

			AI.setbuttonPressedJump (false);
			AI.setbuttonReleasedJump (false);

            float variablePos = Random.Range(-5f, 5f);

			if (transform.position.x - player.transform.position.x < 40f || jumpingToNextPlatform == true) {
				if ((player.transform.position.x < transform.position.x && transform.position.x - player.transform.position.x < 50f) || jumpingToNextPlatform == true) {
					if (jumpingToNextPlatform == false) {
						Move (player.transform.position + new Vector3 (variablePos + 40f, -playerHeight + 3, 0), false); //Add a random x value so it doesn't always stay the same distance
					}
						
					if (savedPlatform.getRight () - transform.position.x < 15f) {
						jumpingToNextPlatform = true;
						Transform targetNode = findNearestPlatform (playerComponent.currentPlatform.GetComponent<Platform> (), true);
						Move (targetNode.position, true);
					}
				} else if (player.transform.position.x > transform.position.x && player.transform.position.x - transform.position.x < 50f || jumpingToNextPlatform == true) {
					if (jumpingToNextPlatform == false) {
						Move (player.transform.position - new Vector3 (variablePos + 40f, -playerHeight + 3, 0), false); //Add a random x value so it doesn't always stay the same distance
					}

					if (transform.position.x - savedPlatform.getLeft () < 15f) {
						jumpingToNextPlatform = true;
						Transform targetNode = findNearestPlatform (playerComponent.currentPlatform.GetComponent<Platform> (), false);
						Move (targetNode.position, true);
					} 
				}
			} else {
				Move (player.transform.position + new Vector3 (variablePos + 40f, -playerHeight + 3, 0), false); //Add a random x value so it doesn't always stay the same distance
			}
		} else if (AI.currentPlatform != playerComponent.currentPlatform) {
			bool onNeighbourPlatform = false;

			foreach (Transform neighbourPlatform in AI.currentPlatform.GetComponent<Platform>().neighbours) {
				if (neighbourPlatform == playerComponent.currentPlatform) {
					onNeighbourPlatform = true;
					break;
				}
			}

			//On a neighbouring platform to the Player
			if (onNeighbourPlatform == true) {
				path.Clear ();
				hasPath = false;
				target = null;
				AI.setbuttonPressedJump (false);
				AI.setbuttonReleasedJump (false);

				float variablePos = Random.Range(-5f, 5f);

				if (player.transform.position.x < transform.position.x && transform.position.x - player.transform.position.x < 30f) { //If within a certain distance stop
					AI.setMovementAxis (new Vector2 (0, 0));
				} else if (player.transform.position.x > transform.position.x && player.transform.position.x - transform.position.x < 30f) { //If within a certain distance stop
					AI.setMovementAxis (new Vector2 (0, 0));
				} else if (player.transform.position.x < transform.position.x && transform.position.x - player.transform.position.x > 30f) { //If not within a certain distance continue
					//nodes[1] represents the second(last) node on platform
					if (transform.position.x - savedPlatform.getLeft () < 15f) {
						Move (playerComponent.currentPlatform.GetComponent<Platform> ().nodes [1].transform.position, true);
					} else {
						Move (playerComponent.currentPlatform.GetComponent<Platform> ().nodes [1].transform.position, false);
					}
				} else if (player.transform.position.x > transform.position.x && player.transform.position.x - transform.position.x > 30f) { //If not within a certain distance continue
					//nodes[0] represents the first node on platform
					if (savedPlatform.getRight () - transform.position.x < 15f) {
						Move(playerComponent.currentPlatform.GetComponent<Platform> ().nodes[0].transform.position, true);
					} else {
						Move(playerComponent.currentPlatform.GetComponent<Platform> ().nodes[0].transform.position, false);
					}
				}
			} else { //target represents a node on the platform
				if (!hasPath) {
					path = pathFinder.FindShortestPath (playerComponent);
					target = path [0];
					path.RemoveAt (0);
					hasPath = true;
				}

				if (target != null && target.transform.parent == AI.currentPlatform) {
					AI.setbuttonPressedJump (false);
					AI.setbuttonReleasedJump (false);
					WalkOnPlatform ();
				} else if (target != null && target.transform.parent != AI.currentPlatform) {
					WalkOnPlatform();
				} else {
					//ReCalcPath ();
					AI.setMovementAxis (new Vector2 (0, 0));
				}
			}
		}*/
	}

	private Transform findNearestPlatform (Platform platform, bool right) {
		List<Transform> rightSide = new List<Transform>();
		List<Transform> leftSide = new List<Transform>();
		Transform targetPlatform;
		Transform targetNode;

		foreach (Transform neighbour in platform.neighbours) {
			if (neighbour.transform.position.x > platform.transform.position.x) {
				rightSide.Add (neighbour);
			} else if (neighbour.transform.position.x < platform.transform.position.x) {
				leftSide.Add (neighbour);
			}
		}

		if (right == true) {
			if (rightSide.Count != 0) {
				targetPlatform = getRandomPlatform (rightSide);
				targetNode = targetPlatform.GetComponent<Platform> ().nodes [0];
			} else {
				targetNode = platform.nodes [0];
			}
		} else {
			if (leftSide.Count != 0) {
				targetPlatform = getRandomPlatform (leftSide);
				targetNode = targetPlatform.GetComponent<Platform> ().nodes [1];
			} else {
				targetNode = platform.nodes [1];
			}
		}

		return targetNode;
	}

	private Transform getRandomPlatform (List<Transform> platforms) {
		int index = (int)Random.Range (0.5f, platforms.Count  - 0.5f);

		return platforms [index];
	}

	private void WalkOnPlatform () {
		if(Mathf.Abs(target.transform.position.x - transform.position.x) < 0.5f && target.transform.parent == AI.currentPlatform) {

			if (path.Count == 0) {
				target = null;
				hasPath = false;
				return;
			}

			target = path [0];
			path.RemoveAt (0);
		}

		Move (target.transform.position, true);
	}

	void Move(Vector3 target, bool canJump) {
		if (target == null) {
			return;
		}

		if (target.x < transform.position.x) {
            AI.setMovementAxis (new Vector2 (-1, 1));
		} else {
			AI.setMovementAxis (new Vector2 (1, 1));
		}
		//Nodes are 3 units above the ground but I added 4 because the player isn't always touching the ground
		if ((canJump && Mathf.Abs (target.x - transform.position.x) < 25f && target.y > transform.position.y - AIHeight + 4)) {
			JumpingHelper ();
		} else {
			amountOfTimePassed = 0f;
			firstStep = true;
			secondStep = false;
			thirdStep = false;
			finalStep = false;
			once = false;
		}
	}

	private float amountOfTimePassed = 0f;
	private bool firstStep = true;
	private bool secondStep = false;
	private bool thirdStep = false;
	private bool finalStep = false;
	private bool once = false;
	public void JumpingHelper() {
		amountOfTimePassed += Time.deltaTime;

		if (firstStep) {
			//Debug.Log ("1 ");

			if (!once) {
				AI.setbuttonPressedJump (true);
				once = true;
			} else {
				AI.setbuttonPressedJump (false);
			}
			AI.setbuttonReleasedJump (false);

			if (amountOfTimePassed > 0.3f) {
				firstStep = false;
				secondStep = true;
				once = false;
			}
		} else if (secondStep) {
			//Debug.Log ("2 ");

			if (!once) {
				AI.setbuttonReleasedJump (true);
				AI.setbuttonPressedJump (false);
				once = true;
			} else {
				AI.setbuttonReleasedJump (false);
				AI.setbuttonPressedJump (false);
				secondStep = false;
				thirdStep = true;
				once = false;
			}
		} else if (thirdStep) {
			//Debug.Log ("3 ");

			if (!once) {
				AI.setbuttonPressedJump (true);
				once = true;
			} else {
				AI.setbuttonPressedJump (false);
			}

			AI.setbuttonReleasedJump (false);
			if (amountOfTimePassed > 0.7f) {
				thirdStep = false;
				once = false;
				finalStep = true;
			}
		} else {
			AI.setbuttonPressedJump (false);
			AI.setbuttonReleasedJump (true);
			finalStep = false;
			firstStep = true;
			amountOfTimePassed = 0f;
		}
	}

	enum States {Follow, Disregard, SamePlatform};
}