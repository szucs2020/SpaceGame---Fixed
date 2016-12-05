/*
 * Network.cs
 * Authors: Christian
 * Description: This is the partially implemented low level networking code I will be using for the chat system
 */
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;

public class Network : NetworkBehaviour {

	private int maxConnections = 10;
	private int socketPort = 8888;

	private int reliableChannelID;
	private int socketID;
	private int connectionID;

	// Use this for initialization
	void Start () {
		
		NetworkTransport.Init();
		ConnectionConfig config = new ConnectionConfig();
		reliableChannelID = config.AddChannel(QosType.Reliable);
		HostTopology top = new HostTopology(config, maxConnections);

		socketID = NetworkTransport.AddHost(top, socketPort);
		Debug.Log("Socket Opened with ID: " + socketID);
	}

	void Destroy(){
		NetworkTransport.Shutdown();
	}

	public void Connect(){
		byte error;
		connectionID = NetworkTransport.Connect(socketID, "localhost", socketPort, 0, out error);
		Debug.Log("Connected to server. ConnectionId: " + connectionID);
	}

	public void SendSocketMessage(string message) {
		byte error;
		byte[] buffer = new byte[1024];
		Stream stream = new MemoryStream(buffer);
		BinaryFormatter formatter = new BinaryFormatter();
		formatter.Serialize(stream, message);

		int bufferSize = 1024;

		NetworkTransport.Send(socketID, connectionID, reliableChannelID, buffer, bufferSize, out error);
	}
	
	// Update is called once per frame
	void Update () {
		
		int recHostId;
		int recConnectionId;
		int recChannelId;
		byte[] recBuffer = new byte[1024];
		int bufferSize = 1024;
		int dataSize;
		byte error;

		NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostId, out recConnectionId, 
			out recChannelId, recBuffer, bufferSize, out dataSize, out error);

		switch (recNetworkEvent) {

		case NetworkEventType.Nothing:
			break;

		case NetworkEventType.ConnectEvent:
			Debug.Log("EVENT: incoming connection event received");
			break;

		case NetworkEventType.DataEvent:
			Stream stream = new MemoryStream(recBuffer);
			BinaryFormatter formatter = new BinaryFormatter();
			string message = formatter.Deserialize(stream) as string;
			Debug.Log("EVENT: incoming message event received: " + message);
			break;

		case NetworkEventType.DisconnectEvent:
			Debug.Log("EVENT: remote client event disconnected");
			break;
		}

	}
}
