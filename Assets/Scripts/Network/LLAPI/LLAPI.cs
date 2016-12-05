using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LLAPI : MonoBehaviour {
    int myReliableChannelId;
    int maxConnections = 10;

    // Use this for initialization
    void Start () {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();

        myReliableChannelId = config.AddChannel(QosType.Reliable);
        HostTopology topology = new HostTopology(config, maxConnections);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
