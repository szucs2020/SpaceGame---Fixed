/*
 * Heap.cs
 * Authors: Lajos Polya
 * Description: This is a Heap which is used by the A* (Star) algorithm
 */
using UnityEngine;
using System.Collections;

public class Heap {
	public Node[] minHeap;
	private int length;
	private const int maxLength = 100;

	public void Init () {
		
		length = 0;
		minHeap = new Node[maxLength];
	}

	private void TestMethod() {
		Node[] elements = new Node[16];
		Init ();
		for (int i = 1; i <= 16; i++) {
			elements [i - 1] = GameObject.Find ("W" + i.ToString ()).GetComponent<Node>();
			elements [i - 1].setF(Random.Range (1, 100));
			Insert (elements[i - 1]);
		}

		Debug.Log ("\n");
		Print();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();

		Extract ();
		Debug.Log ("\n");
		Print ();
	}

	/*void Start() {
		TestMethod ();
	}*/

	public void Insert(Node node) {
		
		if (length == 0) {
			minHeap [1] = node;
		} else {
			minHeap [length+1] = node;

			int i = length + 1;
			Node t = null;

			while (i > 1 && minHeap [i].getF() < minHeap [i / 2].getF()) {

				t = minHeap [i / 2];
				minHeap [i / 2] = minHeap[i];
				minHeap [i] = t;
				i = i / 2;
			}
		}
		length++;
	}

	public Node Extract() {
		int i = 1;
		Node t = null;
		Node e = minHeap [i];

		if (length == 0) {
			return null;
		}

		minHeap [i] = minHeap [length];
		minHeap [length] = null;
		length--;

		while (length >= 2 * i) {
			t = null;
			if (minHeap [i].getF() > minHeap [2 * i].getF()) {
				if (length >= 2 * i + 1) {
					if (minHeap [2 * i + 1].getF() < minHeap [2 * i].getF()) {
						t = minHeap [2 * i + 1];
						minHeap [2 * i + 1] = minHeap [i];
						minHeap [i] = t;
						i = 2 * i + 1;
					} else {
						t = minHeap [2 * i];
						minHeap [2 * i] = minHeap [i];
						minHeap [i] = t;
						i = 2 * i;
					}
				} else {
					t = minHeap [2 * i];
					minHeap [2 * i] = minHeap [i];
					minHeap [i] = t;
					i = 2 * i;
				}
			} else {
				if (length >= 2 * i + 1) {
					if (minHeap [2 * i + 1].getF() < minHeap [i].getF()) {
						t = minHeap [2 * i + 1];
						minHeap [2 * i + 1] = minHeap [i];
						minHeap [i] = t;
						i = 2 * i + 1;
					} else {
						break;
					}
				} else {
					break;
				}
			}
		}
		return e;
	}

	public int GetLength() {
		return length;
	}

	public bool inHeap(Node node) {
		for(int i = 1; i <= length; i++) {
			if (minHeap [i] == node) {
				return true;
			}
		}

		return false;
	}

	public void Print() {
		for(int i = 1; i <= length; i++) {
			Debug.Log (minHeap[i].name);
		}
	}
}
