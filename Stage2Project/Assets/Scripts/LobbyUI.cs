using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {

    [SerializeField]
    private Text status;
    [SerializeField]
    private Text hosting;

    [SerializeField]
    public Button startBtn;

    private GameNetwork net;
    private Canvas currentPanel;
    [SerializeField]
    public Canvas mainPanel;
    [SerializeField]
    public Canvas playerList;
    [SerializeField]
    public GameObject list;

	// Use this for initialization
	void Start () {
        net = GetComponent<GameNetwork>();
        currentPanel = mainPanel;
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void OnClickHost()
    {
        net.StartHost();
    }

    public void OnClickJoin()
    {
        net.StartClient();
    }

    public void OnClickStart()
    {
        net.StartGame();
    }

    public void UpdateInfo(string status, string hosting)
    {
        this.status.text = status;
        this.hosting.text = hosting;

    }

    public void HideAllLobbyUI(List<LobbyPlayer> clients)
    {
        foreach (LobbyPlayer lp in clients)
        {
            print("oo");
            lp.RpcGameUI();
        }
    }


    public void TransitionTo(Canvas next)
    {
        currentPanel.enabled = false;
        currentPanel = next;
        currentPanel.enabled = true;

        if (next == playerList)
        {
            foreach (LobbyPlayer player in net.GetPlayers())
            {
                if (!player.isServer)
                    continue;
                player.RpcToggleStartBtn(true);
            }
        }
    }

}
