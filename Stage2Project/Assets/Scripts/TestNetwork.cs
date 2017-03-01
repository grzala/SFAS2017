using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestNetwork : NetworkManager {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //set game manager as parent while instantiating
        GameObject game = GameObject.Find("Game");
        var player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity, game.transform);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        game.GetComponent<GameManager>().players.Add(player.GetComponent<Player>());
    }
}
