/*
 * Queue.cs
 * Authors: Lajos Polya
 * Description: This is a Queue which stores nodes
 */
using UnityEngine;
using System.Collections;

public class Queue {
	private int length;
	private Node[] queue;
	private const int maxLength = 100;

	public void Init() {
		length = 0;
		queue = new Node[maxLength];
	}

	public void Enqueue(Node node) {
		
		if (length == 0) {
			queue [0] = node;
		} else {
			if (length == 100) {
				throw new UnityException ("QUEUE FULL");
			}

			for (int i = length; i > 0; i--) {
				queue [i] = queue [i - 1];
			}
			queue [0] = node;
		}

		length++;
	}

	public Node Dequeue() {

		Node r = null;

		if (length == 0) {
			return null;
		}

		r = queue [0];

		for (int i = 0; i < length; i++) {
			queue [i] = queue [i + 1];
		}
		queue [length] = null;

		length--;

		return r;
	}

	public int Length() {
		return length;
	}

	private void Test() {

		Node[] elements = new Node[16];
		for (int i = 1; i <= 16; i++) {
			elements [i - 1] = new Node ();
			elements [i - 1].setF(i);
			Enqueue (elements[i - 1]);
		}

		Print ();

		Dequeue ();
		Print();

		Dequeue ();
		Print();

		Dequeue ();
		Print();

		Dequeue ();
		Print();

		Dequeue ();
		Print();

		Dequeue ();
		Print();

		Dequeue ();
		Print();

		Dequeue ();
		Print();
	}


	public void Print() {
		for (int i = 0; i < length; i++) {
			Debug.Log (queue[i].transform.position.x);
		}
	}
}
