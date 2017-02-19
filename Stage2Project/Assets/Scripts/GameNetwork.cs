using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameNetwork : NetworkManager {

	// Use this for initialization
	void Start () {
        
	}


    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //set game manager as parent while instantiating
        var player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity, GetComponent<GameManager>().transform);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        GetComponent<GameManager>().players.Add(player.GetComponent<Player>());

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
