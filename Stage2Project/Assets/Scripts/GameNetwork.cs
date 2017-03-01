using System.Collections;
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

        lp.colorIndex = NextAvailableColor(0);

        players.Add(lp);
        connections.Add(conn, playerControllerId);

        NetworkServer.SpawnWithClientAuthority(lp.gameObject, conn);

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
	
	// Update is called once per frame
	void Update () {

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

        ServerChangeScene("Game");
    }

    public void QuitServer() 
    {
        adminId = -1;
        players = new List<LobbyPlayer>();
        connections = new Dictionary<NetworkConnection, short>();
        StopHost();
    }

    public override void OnServerSceneChanged(string SceneName)
    {
        if (SceneName == "Game")
        {
            int i = 1;
            foreach (KeyValuePair<NetworkConnection, short> pair in connections)
            {
                GameObject game = GameObject.Find("Game");
                var player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity, game.transform);

                LobbyPlayer lp = GetLobbyPlayerByConnectionId(pair.Key.connectionId);
                print(lp.colorIndex);

                //player.GetComponent<Renderer>().material = lp.mats[lp.colorIndex];

                NetworkServer.AddPlayerForConnection(pair.Key, player, pair.Value);

                player.GetComponent<Renderer>().material = player.GetComponent<Player>().mats[lp.colorIndex];
                player.GetComponent<Player>().RpcSetMaterial(lp.colorIndex);
                player.GetComponent<Player>().name += " " + i;

                game.GetComponent<GameManager>().players.Add(player.GetComponent<Player>());
                i++;
            }

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

    public void RemovePlayer(int connectionId)
    {
        bool destroy = false;
        int iToDestroy = 0;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].connectionId == connectionId)
            {
                iToDestroy = i;
                destroy = true;
            }
        }

        LobbyPlayer p = players[iToDestroy];
        players.RemoveAt(iToDestroy);
        Destroy(p.gameObject);
    }
}
