/*
 * Platform.cs
 * Authors: Lajos Polya
 * Description: This script contains information about the platform it represents
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Platform : MonoBehaviour {
	
	public Transform[] neighbours;
	public List<Transform> nodes;
	public Transform[] portals;

	private float length;
	private float left;
	private float right;

	void Start() {
		MeshRenderer renderer = transform.GetComponent<MeshRenderer> ();
		length = renderer.bounds.size.x;

		left = transform.position.x - renderer.bounds.size.x / 2;
		right = transform.position.x + renderer.bounds.size.x / 2;
	}

	public float getLeft() {
		return left;
	}

	public float getRight() {
		return right;
	}
}
