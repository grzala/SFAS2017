﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameNetwork : NetworkManager {

    [SerializeField]
    private GameObject LobbyPlayerPrefab;

    private int adminId = -1;
    private List<LobbyPlayer> players = new List<LobbyPlayer>();
    private Dictionary<NetworkConnection, short> connections = new Dictionary<NetworkConnection, short>();

    private LobbyUI UI;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        UI = GetComponent<LobbyUI>();
        UI.UpdateInfo("offline", "none");
	}

    public List<LobbyPlayer> GetPlayers()
    {
        return players;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        /*
            //set game manager as parent while instantiating
            GameObject game = GameObject.Find("Game");
            var player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity, game.transform);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

            game.GetComponent<GameManager>().players.Add(player.GetComponent<Player>());
        */
        print("adding a player");
        LobbyPlayer lp = (LobbyPlayer)Instantiate(LobbyPlayerPrefab).GetComponent<LobbyPlayer>();
        lp.transform.SetParent(UI.list.transform, false);

        lp.connectionId = conn.connectionId;

        if (adminId < 0 || players.Count < 1)
        {
            adminId = lp.connectionId;
        }

        players.Add(lp);
        connections.Add(conn, playerControllerId);

        NetworkServer.Spawn(lp.gameObject);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame() {
        if (!NetworkServer.active)
            return;

        ServerChangeScene("Game");
    }

    public override void OnServerSceneChanged(string SceneName)
    {
        if (SceneName == "Game")
        {
            foreach (KeyValuePair<NetworkConnection, short> pair in connections)
            {
                GameObject game = GameObject.Find("Game");
                var player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity, game.transform);

                NetworkServer.AddPlayerForConnection(pair.Key, player, pair.Value);

                game.GetComponent<GameManager>().players.Add(player.GetComponent<Player>());
            }

            GetComponent<LobbyUI>().HideAllLobbyUI(players);
        }
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        UI.UpdateInfo("hosting", networkAddress);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        UI.TransitionTo(UI.playerList);

        if (!NetworkServer.active)
        {
            UI.UpdateInfo("client", networkAddress);
        }
    }
}
