using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/* This script is responsible for all networking
 * It Controls the Game and the lobby
 * I found the default example Lobby in asset store,
 * however I decided its very confusing and limitating,
 * so I decided to create my own lobby
 * 
 * This may not have been a good idea
 * Note to self: When using a given game engine for the first time, stick to singleplayer */

/* every connection spawns a LobbyPlayer - information on connection,
 * player name, color and readiness. When game starts, All lobby players spawn a
 * game player */

public class GameNetwork : NetworkManager {

    [SerializeField]
    private GameObject LobbyPlayerPrefab;

    private int adminId = -1;
    private List<LobbyPlayer> players = new List<LobbyPlayer>();
    private Dictionary<NetworkConnection, short> connections = new Dictionary<NetworkConnection, short>();

    private const int MAX_PLAYERS = 4;

    private LobbyUI UI;

    private bool inGame = false;

	// Use this for initialization
	void Start () 
    {
        DontDestroyOnLoad(gameObject);
        UI = GetComponent<LobbyUI>();
        UI.UpdateInfo("offline", "none");
	}

    public List<LobbyPlayer> GetPlayers()
    {
        return players;
    }

    public LobbyPlayer GetLocalPlayer()
    {
        foreach (LobbyPlayer p in players)
        {
            print(p.connectionId);
        }
        return null;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        LobbyPlayer lp = (LobbyPlayer)Instantiate(LobbyPlayerPrefab).GetComponent<LobbyPlayer>();
        lp.transform.SetParent(UI.list.transform, false);

        lp.connectionId = conn.connectionId;

        if (adminId < 0 || players.Count < 1)
        {
            adminId = lp.connectionId;
        }

        lp.colorIndex = NextAvailableColor(0);

        players.Add(lp);
        connections.Add(conn, playerControllerId);
        NetworkServer.SpawnWithClientAuthority(lp.gameObject, conn);
        lp.RpcGetNameFromInput();
       
    }

    public int NextAvailableColor(int index)
    {
        while (!IsColorAvailable(index))
        {
            index++;
            if (index >= LobbyPlayer.colors.Length)
            {
                index = 0;
            }
        }

        return index;
    }

	private bool CanAddPlayer(LobbyPlayer lp)
	{
        return !inGame && players.Count < MAX_PLAYERS && IsNameAvailable(lp.name);
	}

    //Color can be used by at most one player
    private bool IsColorAvailable(int index)
    {
        bool avail = true;

        foreach (LobbyPlayer lp in players)
        {
            if (lp.colorIndex == index)
            {
                avail = false;
                break;
            }
        }

        return avail;
    }

	private bool IsNameAvailable(string name)
	{
		bool avail = true;

		foreach (LobbyPlayer lp in players)
		{
			if (lp.name == name)
			{
				avail = false;
				break;
			}
		}

		return avail;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!NetworkServer.active)
            return;
        
        UI.ToggleStartBtn(PlayersReady());

	}

    private bool PlayersReady()
    {
        if (players.Count < 1)
            return false;

        bool ready = true;

        foreach (LobbyPlayer p in players) 
        {
            if (!p.ready)
            {
                ready = false;
                break;
            }
        }

        return ready;
    }

    public void StartGame() {
        if (!NetworkServer.active)
            return;

        inGame = true;
        ServerChangeScene("Game");

    }

    public void QuitServer() 
    {
        adminId = -1;
        players.Clear();
        connections.Clear();
        StopHost();
        StopClient();
    }

    public override void OnServerSceneChanged(string SceneName)
    {
        if (SceneName == "Game")
        {
            int i = 1;
            GameManager game = GameObject.Find("Game").GetComponent<GameManager>();
            foreach (KeyValuePair<NetworkConnection, short> pair in connections)
            {
                var player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity, game.transform);

                LobbyPlayer lp = GetLobbyPlayerByConnectionId(pair.Key.connectionId);

                //player.GetComponent<Renderer>().material = lp.mats[lp.colorIndex];

                NetworkServer.AddPlayerForConnection(pair.Key, player, pair.Value);

                player.GetComponent<Renderer>().material = player.GetComponent<Player>().mats[lp.colorIndex];
                player.GetComponent<Player>().materialIndex = lp.colorIndex;
                player.GetComponent<Player>().RpcSetMaterial(lp.colorIndex);
                string name = lp.name;
                player.GetComponent<Player>().UpdateName(name);
                player.GetComponent<Player>().RpcSetName(name);

                game.players.Add(player.GetComponent<Player>());
                i++;
            }
            game.SetPlayerPositions();

            GetComponent<LobbyUI>().HideAllLobbyUI(players);
        }
    }

    public LobbyPlayer GetLobbyPlayerByConnectionId(int conId)
    {
        foreach (LobbyPlayer p in players)
        {
            if (p.connectionId == conId)
                return p;
        }

        return null;
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

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        RemovePlayer(conn.connectionId);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RemovePlayer(conn.connectionId);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopClient();
        UI.TransitionTo(UI.mainPanel);
    }

    public override void OnStopServer()
    {
        ClearPlayers();
    }

    public override void OnStopClient()
    {
        //ClearPlayers();
    }

    public void ClearPlayers()
    {
        foreach (LobbyPlayer lp in players)
        {
            Destroy(lp.gameObject);
        }
        players.Clear();
        connections.Clear();
    }

    public void RemovePlayer(int connectionId)
    {

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].connectionId == connectionId)
            {
                LobbyPlayer p = players[i];
                players.RemoveAt(i);
                NetworkServer.Destroy(p.gameObject);
                break;
            }
        }

        foreach (NetworkConnection key in connections.Keys)
        {
            if (key.connectionId == connectionId)
            {
                key.Disconnect();
                connections.Remove(key);
                break;
            }
        }

    }
}
