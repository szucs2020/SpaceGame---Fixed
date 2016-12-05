/*
 * AStar.cs
 * Authors: Lajos Polya
 * Description: This script contains the A* (Star) algorithm which is the path finding algorithm
 * which finds a path from one node to another node
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar : MonoBehaviour {
	public Node startNode;
	private Node currentNode;
	public Node target;


	public List<Node> nodes;
	public Node[] closedNodes;

	Heap open;

	private Player AI;

	void Start() {
		AI = transform.GetComponent<Player> ();
	}

	private void StartHelper () {
		List<GameObject> nodeList = GameObject.Find ("Platforms").GetComponent<PathGen> ().ObjectList;

		nodes = new List<Node> ();
		for (int i = 0; i < nodeList.Count; i++) {
			nodes.Add (nodeList[i].GetComponent<Node> ());
		}

		if (AI.currentPlatform != null) {
			Platform platform = AI.currentPlatform.GetComponent<Platform> ();
			if (Mathf.Abs ((transform.position - platform.nodes [0].transform.position).magnitude) < Mathf.Abs ((transform.position - platform.nodes [1].transform.position).magnitude)) {
				startNode = platform.nodes [0].GetComponent<Node> ();
			} else {
				startNode = platform.nodes [1].GetComponent<Node> ();
			}
		} else {
			startNode = GetClosestNodeToPlayer (transform);
		}
	}

	private Node GetClosestNodeToPlayer(Transform member) {
		Transform Spawns = member.Find ("spawn");
		RaycastHit2D Hit;
		Hit = Physics2D.Raycast (new Vector2(Spawns.GetChild(0).position.x, Spawns.GetChild(0).position.y - 4), (Spawns.GetChild(0).rotation * Vector2.right) * 50, 50f);

		Platform platform = Hit.transform.GetComponent<Platform> ();
		if (Mathf.Abs ((member.position - platform.nodes [0].transform.position).magnitude) < Mathf.Abs ((member.position - platform.nodes [1].transform.position).magnitude)) {
			return platform.nodes [0].GetComponent<Node> ();
		} else {
			return platform.nodes [1].GetComponent<Node> ();
		}
	}

	public List<Node> FindShortestPath(Player targetPlayer) {
		StartHelper ();

		float shortestDist = float.MaxValue;
		float dist = float.MaxValue;

		if (targetPlayer.currentPlatform != null) {
			Platform platform = targetPlayer.currentPlatform.GetComponent<Platform> ();
			if (Mathf.Abs ((targetPlayer.transform.position - platform.nodes [0].transform.position).magnitude) < Mathf.Abs ((targetPlayer.transform.position - platform.nodes [1].transform.position).magnitude)) {
				target = platform.nodes [0].GetComponent<Node> ();
			} else {
				target = platform.nodes [1].GetComponent<Node> ();
			}
		} else {
			target = GetClosestNodeToPlayer (targetPlayer.transform);
		}

		//Initialize each node
		for(int i = 0; i < nodes.Count; i++) {
			nodes [i].Init (target);
		}

		//Closed nodes were used for testing purposes but they might have use later
		for(int i = 0; i < closedNodes.Length; i++) {
			if (closedNodes [i] != null) {
				closedNodes [i].setClosed(true);
				closedNodes [i].setH(float.MaxValue);
				closedNodes [i].setColour(Color.white);
			}
		}

		open = new Heap ();
		open.Init ();
		//Initializing First Node
		currentNode = startNode;
		currentNode.setG(0f);
		currentNode.setF(currentNode.getH());
		currentNode.clearParent();
		currentNode.setColour(Color.red);
		open.Insert (currentNode);

		while(open.GetLength() > 0) {

			currentNode = open.Extract ();
			currentNode.setClosed(true);
			currentNode.setOpen(false);

			if (currentNode == null) {
				break;
			}

			for (int i = 0; i < currentNode.neighbour.Count; i++) {
				
				if (currentNode.neighbour [i] != null) {

					if (currentNode.neighbour [i].isTarget == true && currentNode.neighbour[i].getClosed() == false) {

						currentNode.neighbour [i].setParent(currentNode);
						currentNode.setColour(Color.blue);
						ColourPath (currentNode.neighbour [i]);
						List<Node> path = new List<Node>();

						//Add Target and CurrentNode to Queue before traversing backwards
						path.Insert (0, target);
						target.inPath = true;
						path.Insert (0, currentNode);
						currentNode.inPath = true;
						while (currentNode.getParent () != null) {
							path.Insert (0, currentNode.getParent ());
							currentNode = currentNode.getParent ();
							currentNode.inPath = true;
						}

						if (path.Count > 2 && path[0].transform.parent == path[1].transform.parent) {
							path.RemoveAt (0);
						}
						return path;
					} else if (currentNode.neighbour [i].getClosed() == true) {
						continue;
					}

					float tentativeG = Vector2.Distance(currentNode.neighbour [i].transform.position, currentNode.transform.position) + currentNode.getG();

					if (currentNode.neighbour [i].getOpen() == false) {

						currentNode.neighbour [i].setOpen(true);
					} else if (tentativeG >= currentNode.neighbour [i].getG()) {
						//This node neighbours a closer node
						continue;
					}

					currentNode.neighbour [i].setParent(currentNode);
					currentNode.neighbour [i].setG(tentativeG);
					currentNode.neighbour [i].setF(currentNode.neighbour [i].getG() + currentNode.neighbour [i].getH());
					currentNode.neighbour [i].setColour(Color.blue);

					//You have to add it to the Open Heap here because the Fvalue only gets calculated right before this
					if (open.inHeap(currentNode.neighbour [i]) == false && currentNode.neighbour [i].getOpen () == true && currentNode.neighbour [i].getClosed () == false) {
						open.Insert (currentNode.neighbour [i]);
					}
				}
			}
		}
			
		List<Node> backupPath = new List<Node> ();
		backupPath.Insert (0, startNode);
		return backupPath;
	}


	/*Starting with the target node it travels through each parent
	 * and colours it until it reaches the start node*/
	private void ColourPath(Node target) {
		Node current = target;

		while (current.getParent() != null) {
			current.setColour(Color.green);
			current = current.getParent();
		}
		startNode.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
		target.setColour(Color.red);
	}
}
