/*
 * Node.cs
 * Authors: Lajos Polya
 * Description: This script describes a node and all of its properties.  Nodes are the edges of a tree when creating a path
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {
	public List<Node> neighbour;
	public float g;  //Movement cost to neighbour;
	public float h;  //Heuristic; Distance to target node
	public float f;  // g + h

	public Node parent;

	private bool open;
	private bool closed;
	public bool isTarget;
	public bool inPath;

	// Use this for initialization
	public void Init (Node target) {

		parent = null;
		g = float.MaxValue;
		f = float.MaxValue;
		closed = false;
		open = false;
		inPath = false;
		transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;

		setH(target.transform.position, transform.position);

		if (this == target) {

			transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
			isTarget = true;
		} else {
			isTarget = false;
		}
	}

	public void setParents () {
		for (int i = 0; i < 4; i++) {
			if (neighbour [i] != null) {
				neighbour [i].parent = this;
			}
		}
	}

	public void setF(float f) {
		this.f = f;
	}

	public float getF() {
		return f;
	}

	public void setG(float g) {
		this.g = g;
	}

	public void setG(Vector3 x, Vector3 y) {
		g = Vector2.Distance (x, y);
	}

	public float getG() {
		return g;
	}

	public void setH(float h) {
		this.h = h;
	}

	public void setH(Vector3 x, Vector3 y) {
		h = Vector2.Distance (x, y);
	}

	public float getH() {
		return h;
	}

	public void setOpen(bool open) {
		this.open = open;
	}

	public bool getOpen() {
		return open;
	}

	public void setClosed(bool closed) {
		this.closed = closed;
	}

	public bool getClosed() {
		return closed;
	}

	public void setParent(Node parent) {
		this.parent =  parent;
	}

	public Node getParent() {
		return parent;
	}

	public void clearParent() {
		this.parent = null;
	}

	public bool getInPath() {
		return inPath;
	}

	public void setInPath(bool inPath) {
		this.inPath =  inPath;
	}

	public void setColour(Color colour) {
		transform.GetChild (0).GetComponent<MeshRenderer> ().material.color = colour;
	}
}
