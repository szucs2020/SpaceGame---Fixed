/*
 * PathGen.cs
 * Authors: Lajos Polya
 * Description: This script gets all the nodes on the platforms and connects them to create a tree, which paths can 
 * be formed on by the A* (Star) algorithm.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathGen : MonoBehaviour {

	public GameObject Node;
	public List<GameObject> ObjectList;

	// Use this for initialization
	void Start () {

		ObjectList = new List<GameObject> ();
		GameObject instance;

		foreach (Transform child in transform) {
			
			MeshRenderer renderer = child.GetComponent<MeshRenderer> ();
			Platform platform = child.GetComponent<Platform> ();
			platform.nodes = new List<Transform> ();
			int length = (int)renderer.bounds.size.x;

			//Generates nodes for each individual platform
			//for (int i = length; i > 0; i = i - 5) {

				//If the node is too close to the edge of the platform it will not get generated
				//if(!(length - i < 2) && !(length - i > length - 2)) {
			instance = (GameObject)Instantiate (Node, new Vector3 ((int)child.position.x + 10 - length / 2, (int)child.position.y + 3, 0), Quaternion.identity);
			instance.transform.SetParent (child, true);
			platform.nodes.Add (instance.transform);
			ObjectList.Add(instance);

			instance = (GameObject)Instantiate (Node, new Vector3 ((int)child.position.x - 10 + length / 2, (int)child.position.y + 3, 0), Quaternion.identity);
			instance.transform.SetParent (child, true);
			platform.nodes.Add (instance.transform);
			ObjectList.Add(instance);
				//}

				
			//}


			//Each node in each platform neighbours the nodes next to it
			List<Node> objectNode;
			for (int i = 0; i < platform.nodes.Count; i++) {
				
				platform.nodes [i].name = platform.name + " " + i.ToString();
				platform.nodes [i].GetComponent<Node> ().neighbour = new List<Node> ();
				objectNode = platform.nodes [i].GetComponent<Node> ().neighbour;

				if (i == 0) {
					objectNode.Add (platform.nodes [i + 1].GetComponent<Node>());
				} /*else if(i != 0 && i != platform.nodes.Count - 1) {  //When whole platform has nodes on it
					objectNode.Add (platform.nodes [i + 1].GetComponent<Node>());
					objectNode.Add (platform.nodes [i - 1].GetComponent<Node>());
				} */else if(i == platform.nodes.Count - 1) {//It's dangerous to do Count - 1 since portals are added but...
					objectNode.Add (platform.nodes [i - 1].GetComponent<Node>());///portal are added after this line so it shouldn't matter
				}
			}

			//Add a node for each portal and make the edge node of the platform a neighbour of the portal node
			foreach (Transform portal in child.GetComponent<Platform>().portals) {
				instance = (GameObject)Instantiate (Node, new Vector3 (portal.transform.position.x, (int)child.position.y + 3, 0), Quaternion.identity);
				instance.name = child.name + " " + portal.name;
				instance.transform.SetParent (child, true);
				platform.nodes.Add (instance.transform);
				ObjectList.Add(instance);
				portal.GetComponent<Portal> ().SetNode (instance.GetComponent<Node> ());

				//Add dependent portal as variable
				instance.GetComponent<Node> ().neighbour = new List<Node> ();
				if (portal.transform.position.x < platform.nodes [0].transform.position.x) {
					instance.GetComponent<Node> ().neighbour.Add (platform.nodes [0].GetComponent<Node> ());
					platform.nodes [0].GetComponent<Node> ().neighbour.Add (instance.GetComponent<Node> ());
				} else if (portal.transform.position.x > platform.nodes [1].transform.position.x) {
					instance.GetComponent<Node> ().neighbour.Add (platform.nodes [1].GetComponent<Node> ());
					platform.nodes [1].GetComponent<Node> ().neighbour.Add (instance.GetComponent<Node> ());
				} else {
					Debug.LogError ("A Portal is Missplaced (Not Close Enough to Edge of Platform)");
				}
			}
		}

		//Connects each pair of portals to each other
		foreach (Transform child in transform) {
			foreach (Transform portal in child.GetComponent<Platform>().portals) {
				portal.GetComponent<Portal> ().GetNode ().neighbour.Add (portal.GetComponent<Portal> ().target.GetComponent<Portal> ().GetNode ());
			}
		}

		//Connects the neighbourng platforms
		foreach (Transform child in transform) {

			MeshRenderer renderer = child.GetComponent<MeshRenderer> ();
			Platform platform = child.GetComponent<Platform> ();
			int length = (int)renderer.bounds.size.x;

			foreach (Transform neighour in child.GetComponent<Platform> ().neighbours) {
				MeshRenderer neighbourRenderer = neighour.GetComponent<MeshRenderer> ();
				Platform neighbourPlatform = neighour.GetComponent<Platform> ();
				int neighbourLength = (int)neighbourRenderer.bounds.size.x;

				//If the platform is to the right or left of its neighbour connect its respective nodes
				if (child.position.x + length / 2 < neighour.position.x - neighbourLength / 2) {
					platform.nodes [1].GetComponent<Node> ().neighbour.Add(neighbourPlatform.nodes [0].GetComponent<Node> ());
				} else if (child.position.x - length / 2 > neighour.position.x + neighbourLength / 2) {

					platform.nodes [0].GetComponent<Node> ().neighbour.Add(neighbourPlatform.nodes [1].GetComponent<Node> ());
				}

				//If you can go through the platform then connect respective nodes
				if (child.tag == "Through" || neighour.tag == "Through") {
					for (int i = 0; i < platform.nodes.Count; i++) {
						for (int j = 0; j < neighbourPlatform.nodes.Count; j++) {
							if (platform.nodes[i].transform.position.x - 3f <= neighbourPlatform.nodes[j].transform.position.x && neighbourPlatform.nodes[j].transform.position.x <= platform.nodes[i].transform.position.x + 3f) {
								platform.nodes [i].GetComponent<Node> ().neighbour.Add (neighbourPlatform.nodes[j].GetComponent<Node> ());
							}
						}
					}
				}
			}
		}
	}	
}