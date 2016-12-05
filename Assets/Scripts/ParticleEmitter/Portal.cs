/*
 * Portal.cs
 * Authors: Lorant
 * Description: This script controls the funtionality of portals
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour {
    public GameObject target;
    public float waitTime = 1f;
    public PortalFragments warpPrefab;
    private List<PortalFragments> warp;
    private float startTime;
    private bool wait = false;
    private Portal targetScript;
    private Vector3 deltaD;
    private int numWarp = 5;

	public Node node;

    // Use this for initialization
    void Start() {
        targetScript = target.GetComponent<Portal>();

        warp = new List<PortalFragments>();
        for (int i = 0; i < numWarp; i++)
        {
            warp.Add((PortalFragments)Instantiate(warpPrefab, transform.position, Quaternion.identity));
            if (i == numWarp - 1)
            {
                warp[i].changeSize = false;
            }
            warp[i].size = 1.2f * (i + 1) / numWarp;
            warp[i].transform.SetParent(transform.GetChild(0), false);
            warp[i].transform.localPosition = new Vector3(0, 0, 0);
        }

		node = null;
    }

    // Update is called once per frame
    void Update()
    { 
        if (Time.time - startTime >= waitTime && wait == true)
        {
            wait = false;
        }
    }

    public void Wait()
    {
        wait = true;
        startTime = Time.time;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (target != null)
        {
            if (!wait)
            {
                if (LayerMask.LayerToName(col.gameObject.layer) != "Ground")
                {
                    deltaD = col.gameObject.transform.position - gameObject.transform.GetChild(0).transform.position;
                    col.gameObject.transform.position = target.transform.position + deltaD;
                    targetScript.Wait();
                }
            }
        }
    }

	public void SetNode(Node node) {
		this.node = node;
	}

	public Node GetNode() {
		return node;
	}
}
