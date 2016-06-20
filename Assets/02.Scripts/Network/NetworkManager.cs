using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

    private const string m_ip = "127.0.0.1";
    private const int m_port = 30000;
    private bool m_useNat = false;
    public GameObject m_playerPrefab = null;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            if (GUI.Button(new Rect(20, 20, 200, 25), "Start Server"))
            {
                Network.InitializeServer(20, m_port, m_useNat);
            }

            if (GUI.Button(new Rect(20, 50, 200, 25), "Connect to Server"))
            {
                Network.Connect(m_ip, m_port);
            }
        }
        else
        {
            if (Network.peerType == NetworkPeerType.Server)
            {
                int count = Network.connections.Length;
                GUI.Label(new Rect(20, 20, 200, 25), "Initialization Server...");
                GUI.Label(new Rect(20, 50, 200, 25), string.Format("Client Count = {0}", count));
            }
            else if (Network.peerType == NetworkPeerType.Client)
            {
                GUI.Label(new Rect(20, 20, 200, 25), "Connected to Server");
            }
        }
    }

    void OnServerInitialized()
    {
        CreatePlayer();
    }

    void OnConnectedToServer()
    {
        CreatePlayer();
    }

    void OnPlayerDisconnected(NetworkPlayer netPlayer)
    {
        Network.RemoveRPCs(netPlayer);
        Network.DestroyPlayerObjects(netPlayer);
    }

    void CreatePlayer()
    {
        if (m_playerPrefab == null)
            return;

        Vector3 pos = new Vector3(Random.Range(-20.0f, 20.0f), 0.0f, Random.Range(-20.0f, 20.0f));
        Network.Instantiate(m_playerPrefab, pos, Quaternion.identity, 0);
    }
}
